using MediCare.Dto;
using MediCare.Helpers.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediCare.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string? GetNameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;

                return name;
            }

            return null;
        }

        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            int userId = 0;
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "sub")?.Value;
                userId = Convert.ToInt32(sid);
                return userId;
            }
            return userId;
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
                    Name = name ?? string.Empty,
                    RoleId = role != null ? (short)Convert.ToInt32(role) : (short)0
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
        public TokenClaimDto GetRolIdandCustTypeIdFromToken(string token)
        {
            TokenClaimDto tokenClaimDto = new TokenClaimDto();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value;

                tokenClaimDto = new TokenClaimDto()
                {
                    RoleId = role != null ? (short)Convert.ToInt32(role) : (short)0,
                };
            }
            return tokenClaimDto;
        }
    }
}
