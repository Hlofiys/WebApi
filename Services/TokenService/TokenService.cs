using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Services.TokenService
{
    public static class TokenService
    {

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

                var now = DateTime.UtcNow.Ticks;
                Console.WriteLine(now);
                Console.WriteLine(tokenDate);
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
    }
}
