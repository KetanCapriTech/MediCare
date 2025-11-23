using MediCare.Dto;
using MediCare.Models;
using Microsoft.IdentityModel.Tokens;

namespace MediCare.Services.Interfaces
{
    public interface IJwtHelper
    {
        string GenrateJwtToken(User user);
        bool ValidateToken(string token , out SecurityToken? securityToken);

        TokenClaimDto GetRolIdFromToken(string token);

        UserDto GetUserFromToken(string token);
    }
}
