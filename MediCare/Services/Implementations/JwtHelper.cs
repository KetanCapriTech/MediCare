using MediCare.Dto;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediCare.Services.Implementations
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public string GenrateJwtToken(User user)
        {
            // to genrate we need below steps
            // get jwt key from configuration appsetting with expireminuetes
            string keyString = _configuration["JwtSettings:Key"];
            var expireMinutesString = _configuration["JwtSettings:AccessTokenExpiry"];
            
            // before proceding further check the key string and exireminuets
            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(expireMinutesString))
            {
                throw new InvalidOperationException("Missing configuration for JWT");
            }

            // then we need key so using encoding utf8 and expremenites in int format
            var key = Encoding.UTF8.GetBytes(keyString);
            var expireMinutes = Convert.ToInt32(expireMinutesString);

            // add clamins 1st id, email, firstnamelast name , role, jti identity
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email , user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            // after claims adding get the issure , audience , subject, expire, signcredentilas
            // in subject i am passing climsIdenty and in paramter our added clamins or created clamins
            // in signing credentilas we creae new signincredentilas by passing symmerticsecuritykey , and security algi type
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JwtSettings:MediCareApi"],
                Audience = _configuration["JwtSettings:Audience"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // then token insilised jwt security toklen handler 
            var tokenHandler = new JwtSecurityTokenHandler();
            //and pass the tokenDescriptor in create token method
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public TokenClaimDto GetRolIdFromToken(string token)
        {
            TokenClaimDto tokenClaimDto = new TokenClaimDto();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;
                // Removed customertypeid claim retrieval

                tokenClaimDto = new TokenClaimDto()
                {
                    RoleId = role != null ? (short)Convert.ToInt32(role) : (short)0,
                    // CustomerTypeId initialization removed
                };
            }
            return tokenClaimDto;
        }

        public UserDto GetUserFromToken(string token)
        {
            UserDto userModel = new UserDto();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;

                userModel = new UserDto
                {
                    Id = userId != null ? Convert.ToInt32(userId) : 0,
                    FirstName = name ?? string.Empty
                };
            }
            return userModel;
        }

        public bool ValidateToken(string token, out SecurityToken? validatedToken)
        {
            ClaimsPrincipal principal;
            validatedToken = null;
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var keyString = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }
            var key = Encoding.UTF8.GetBytes(keyString);
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for immediate expiration check
                };

                principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
