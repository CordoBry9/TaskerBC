using Microsoft.JSInterop;
using System.Text.Json;
using System.Net.Http;
using TaskerBC.Models.Auth;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;


namespace TaskerBC.Services
{
    public class TokenProvider
    {
        private static readonly string _localStorageKey = "taskerAuthToken";

        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        private AccessTokenResponse? _cachedToken;

        public TokenProvider(IHttpClientFactory httpClientFactory, IConfiguration config, IJSRuntime js)
        {
            _http = httpClientFactory.CreateClient();
            _js = js;

            _http.BaseAddress = new Uri(config["ApiUrl"]!);

        }

        public async Task StoreToken(AccessTokenResponse accessToken)
        {
            _cachedToken = accessToken;

            _cachedToken.ExpiresAt ??= DateTime.Now.AddSeconds(_cachedToken.ExpiresIn);

            string json = JsonSerializer.Serialize(_cachedToken);
            await _js.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
        }

        public async Task ClearToken()
        {
            _cachedToken = null;
            await _js.InvokeVoidAsync("localStorage.setItem", _localStorageKey, string.Empty);
        }

        private async Task<AccessTokenResponse?> GetStoredToken()
        {
            string? json = await _js.InvokeAsync<string>("localStorage.getItem", _localStorageKey);

            if (!string.IsNullOrEmpty(json))
            {
                AccessTokenResponse? token = JsonSerializer.Deserialize<AccessTokenResponse>(json);
                return token;
            }    
            else
            {
                return null;
            }
        }

        private async Task<AccessTokenResponse?> RefreshToken()
        {
            _cachedToken ??= await GetStoredToken();
            if (_cachedToken == null) //there is no token to refresh!
            {
                return null;
            }

            RefreshRequest request = new RefreshRequest() { RefreshToken = _cachedToken.RefreshToken };
            HttpResponseMessage response = await _http.PostAsJsonAsync("refresh", request);

            if (response.IsSuccessStatusCode == false)
            {
                return null; //refresh 
            }

            AccessTokenResponse? newToken = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();

            if (newToken == null)
            {
                return null;
            }

            newToken.ExpiresAt = DateTime.Now.AddSeconds(newToken.ExpiresIn);
            await StoreToken(newToken);
            return newToken;
        }

        public async Task<AccessTokenResponse?> GetValidTokenAsync()
        {
            // if we don't have a cached token, get it from local storage

            _cachedToken ??= await GetStoredToken();
            if (_cachedToken == null)
            {
                return null; //the useri s logged out
            }
            //make sure token is not expired
            if (_cachedToken.ExpiresAt < DateTime.Now) 
            {
                //if it is expired, refresh it
                _cachedToken = await RefreshToken();
            }

            //return whatever we have
            return _cachedToken;
        }
    }
}
