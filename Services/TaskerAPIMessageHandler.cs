using System.Data;
using TaskerBC.Models.Auth;
namespace TaskerBC.Services
{
    public class TaskerAPIMessageHandler(TokenProvider tokenProvider) : DelegatingHandler
    {

        private AccessTokenResponse? _accessToken;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(_accessToken == null || _accessToken.ExpiresAt < DateTime.Now) 
            {
                _accessToken = await tokenProvider.GetValidTokenAsync();
            }

            if (_accessToken is not null)
            {
                request.Headers.Authorization = new("Bearer", _accessToken.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
