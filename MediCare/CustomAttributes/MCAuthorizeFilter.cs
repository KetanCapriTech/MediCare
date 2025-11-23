using MediCare.Dto;
using MediCare.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Permissions;

namespace MediCare.CustomAttributes
{
    public class MCAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IJwtHelper _jwtHelper;

        public MCAuthorizeFilter(IJwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //get token from header
            var header = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (header != null || !header.StartsWith("Bearer"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = header.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token)) {
                context.Result = new UnauthorizedResult();
                return;
            }

            TokenClaimDto tokenClaimDto = new TokenClaimDto();

            try
            {
                SecurityToken? validatedToken;

                bool isValid = _jwtHelper.ValidateToken(token, out validatedToken);
                if (!isValid || validatedToken == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                tokenClaimDto = _jwtHelper.GetRolIdFromToken(token);
            }
            catch (Exception ex) {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
