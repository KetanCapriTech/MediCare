using MediCare.Dto;
using Microsoft.IdentityModel.Tokens;

namespace MediCare.Helpers.Interface
{
    public interface IJwtHelper
    {
        int GetUserIdFromToken(string token);
        string? GetNameFromToken(string token);
        UserDto GetUserFromToken(string token);
        bool ValidateToken(string token, out SecurityToken? validatedToken);
        TokenClaimDto GetRolIdandCustTypeIdFromToken(string token);
    }
}
