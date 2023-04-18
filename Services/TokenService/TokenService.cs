using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Services
{
    public class TokenService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public TokenService(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }

        public static JwtSecurityToken? ValidateToken(string token, IConfiguration _configuration)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, GetValidationParameters(), out var decodedToken);
                var jwttoken = (JwtSecurityToken)decodedToken;

                var tokenTicks = jwttoken.Claims.First(x => x.Type == "exp").Value;
                var ticks = long.Parse(tokenTicks);
                var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;

                var now = DateTimeOffset.Now.ToUnixTimeSeconds();
                var valid = ticks >= now;
                if (!valid) throw new SecurityTokenExpiredException("Token expired");
                return jwttoken;
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }
            TokenValidationParameters GetValidationParameters()
            {
                var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
                if (appSettingsToken is null)
                    throw new Exception("AppSettings Token is null!");
                return new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken))
                };
            }
        }
        public ServiceResponse<User>? UserSearch(string token)
        {
            var response = new ServiceResponse<User>();
            JwtSecurityToken? jwttoken;
            if (token is not null)
            {
                jwttoken = TokenService.ValidateToken(token, _configuration);
            }
            else
            {
                response.Success = false;
                return response;
            }
            if (jwttoken is null)
            {
                response.Success = false;
                response.StatusCode = 401;
                return response;
            }
            var userIdString = jwttoken.Claims.First(x => x.Type == "nameid").Value;
            int userId = int.Parse(userIdString);
            var user = _context.Users.ToList().Find(u => u.Id == userId);
            if (user is null)
            {
                response.Success = false;
                return response;
            }
            response.Data = user;
            return response;
        }
    }
}
