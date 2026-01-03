using MediCare.Helpers.Interface;
using MediCareWeb.Services.Interfaces;
using MediCareDto.API;
using MediCareDto.Auth;
using MediCareWeb.Helpers.Interface;

namespace MediCare.Helpers
{
    public class SessionUtilityHelper : ISessionUtilityHelper
    {
        private readonly IUserService _userService;
        private readonly ISessionHelper _sessionHelper;
        public SessionUtilityHelper(IUserService userService, ISessionHelper sessionHelper)
        {
            _userService = userService;
            _sessionHelper = sessionHelper;
        }
        public async Task<bool> SetUserSessionAsync(AuthResponse authResponse)
        {
            try
            {
                SessionDto userSession = new SessionDto();
                var UserInformation = await _userService.GetUserById(authResponse.UserId);

                if (UserInformation != null)
                {
                    userSession = new()
                    {
                        Id = authResponse.UserId,
                        FirstName = UserInformation.FirstName,
                        LastName = UserInformation.LastName,
                        RoleId = UserInformation.RoleId,
                    };
                    _sessionHelper.SetObject("UserSession", userSession);

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> UpdateUserSessionAsync(long userId)
        {
            SessionDto? userSessionold = _sessionHelper.GetObject<SessionDto>("UserSession");

            if (userSessionold == null)
            {
                // Session object is null, cannot update
                return false;
            }

            var UserInformation = await _userService.GetUserById(userId);
            if (UserInformation == null)
            {
                // User information not found, cannot update
                return false;
            }

            var userSession = new SessionDto
            {
                Id = userId,
                FirstName = UserInformation.FirstName,
                LastName = UserInformation.LastName,
                RoleId = UserInformation.RoleId,
            };

            _sessionHelper.Remove("UserSession");
            _sessionHelper.SetObject("UserSession", userSession);
            return true;
        }
    }
}
