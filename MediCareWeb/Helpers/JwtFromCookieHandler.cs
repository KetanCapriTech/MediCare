using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MediCare.Helpers.Interface;
using MediCareDto.API;
using Microsoft.AspNetCore.Http;


namespace MediCare.Helpers
{
    public class JwtFromCookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionHelper _sessionHelper;

        public JwtFromCookieHandler(IHttpContextAccessor httpContextAccessor, ISessionHelper sessionHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionHelper = sessionHelper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1) If this is your login path, don’t add the header:  
            if (request.RequestUri?.AbsolutePath?.EndsWith("/login", System.StringComparison.OrdinalIgnoreCase) == true)
                return await base.SendAsync(request, cancellationToken);

            // 2) Otherwise grab the token from the session:  
            SessionDto? userSession = _sessionHelper.GetObject<SessionDto>("UserSession");

            if (userSession != null)
            {
                var token = userSession.UserToken;
                // 3) If the token is not null or empty, add it to the request headers:
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
