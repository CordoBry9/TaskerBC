using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using TaskerBC.Models.Auth;

namespace TaskerBC.Services
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {

        private static readonly AuthenticationState _defaultUnaethenticatedState =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        private readonly HttpClient _http;
        private readonly TokenProvider _tokenProvider;
        private AuthenticationState _currentState;

        public TokenAuthenticationStateProvider(IHttpClientFactory httpClientFactory, TokenProvider tokenProvider)
        {
            _http = httpClientFactory.CreateClient("TaskerApiClient");
            _tokenProvider = tokenProvider;
            _currentState = _defaultUnaethenticatedState;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AccessTokenResponse? accessToken = await _tokenProvider.GetValidTokenAsync();

            if (accessToken is null)
            {
                _currentState = _defaultUnaethenticatedState;
                return _currentState;
            }
            else if (_currentState.User.Identity?.IsAuthenticated == true)
            {
                return _currentState;
            }
            else
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/me");
                    request.Headers.Authorization = new("Bearer", accessToken.AccessToken);

                    HttpResponseMessage response = await _http.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    Dictionary<string, string> claimDictionary =
                        await response.Content.ReadFromJsonAsync<Dictionary<string, string>>() ?? [];

                    IEnumerable<Claim> claims = claimDictionary.Select(c => new Claim(c.Key, c.Value));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
                    ClaimsPrincipal user = new ClaimsPrincipal(identity);
                    _currentState = new AuthenticationState(user);

                    return _currentState;
                }
                catch
                {
                    _currentState = _defaultUnaethenticatedState;
                    return _currentState;
                }
            }
        }
    }
}
