using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }

        public async Task<ServiceResponse<string>> Login(string username, string password, HttpResponse httpResponse)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                await CreateRefreshToken(user, httpResponse);
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<ServiceResponse<string>> Register(User user, string password, HttpResponse httpResponse)
        {
            var response = new ServiceResponse<string>();
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await CreateRefreshToken(user, httpResponse);
            response.Data = CreateToken(user);
            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(string username, string password)
        {
            var response = new ServiceResponse<bool>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                response.Data = true;
                return response;
            }

            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        private async Task<string> CreateRefreshToken(User user, HttpResponse response)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            
            await SetRefreshToken(tokenHandler.WriteToken(token), response, user);

            return tokenHandler.WriteToken(token);
        }

        private async Task SetRefreshToken(String newRefreshToken, HttpResponse response, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.Now.AddDays(30),
            };
            response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpires = DateTime.Now.AddDays(30);

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public ServiceResponse<string> CheckToken(HttpRequest request)
        {
            var response = new ServiceResponse<string>();
            var token = request.Headers["x-access-token"].ToString();
            JwtSecurityToken? jwttoken;
            if (token is not null){
                jwttoken = TokenService.ValidateToken(token, _configuration);
            }
            else
            {
                response.Success = false;
                return response;
            }
            if(jwttoken is null){
                response.Success = false;
                response.StatusCode = 401;
                return response;
            }
            var name = jwttoken.Claims.First(x => x.Type == "unique_name").Value;
            response.Data = name;
            return response;

        }

        
         public async Task<ServiceResponse<string>> Refresh(HttpRequest request, HttpResponse Httpresponse)
        {
            var response = new ServiceResponse<string>();
            var token = request.Cookies["refreshToken"];
            JwtSecurityToken? jwttoken;
            if (token is not null){
                jwttoken = TokenService.ValidateToken(token, _configuration);
            }
            else
            {
                response.Success = false;
                return response;
            }
            if(jwttoken is null){
                response.Success = false;
                response.StatusCode = 401;
                return response;
            }
            var userId = jwttoken.Claims.First(x => x.Type == "nameid").Value;
            var userIdInt = int.Parse(userId);
            var user = _context.Users.Find(userIdInt);
            if(user is null){
                response.Success = false;
                return response;
            }
            if(user.RefreshToken == token){
               await CreateRefreshToken(user, Httpresponse);
               var AccessToken = CreateToken(user);
               response.Data = AccessToken;
               return response;
            }
            else{
                response.Success = false;
                response.StatusCode = 403;
                return response;
            }
            

        }
    }
}