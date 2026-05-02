using MeterViewMangement.Helpers;
using System.Net.Http.Headers;

namespace MeterViewMangement.Http
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = TokenStorage.Get(_contextAccessor.HttpContext);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
