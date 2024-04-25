using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TaskerBC.Components.Pages;
using TaskerBC.Models;
using TaskerBC.Models.Auth;

namespace TaskerBC.Services
{
    public class TaskerItemService
    {

        //dependency injection in C#
        private readonly IJSRuntime _jsRuntime;

        private readonly HttpClient _http;
        private readonly TokenProvider _tokenProvider;

        public TaskerItemService(IJSRuntime jsRuntime, IHttpClientFactory httpClientFactory, TokenProvider tokenProvider) //constructor
        { 
            _jsRuntime = jsRuntime;
            _tokenProvider = tokenProvider;

            _http = httpClientFactory.CreateClient("TaskerApiClient");
        }
        public async Task SaveTaskerItems(List<TaskerItem> taskerItems)
        {
            string itemsAsString = JsonSerializer.Serialize(taskerItems); 

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "TaskerItems", itemsAsString); 
        }

        public async Task<List<TaskerItem>> GetTaskerItems()
        {
            List<TaskerItem> taskerItems = await _http.GetFromJsonAsync<List<TaskerItem>>("api/TaskerItems") ?? [];

            return taskerItems;
        }

        public async Task<bool> LoginAsync(LoginRequest loginRequest) //confirm log in through request and response
        {
            HttpResponseMessage loginResponse = await _http.PostAsJsonAsync("login", loginRequest); //get the login response to our request, using postasjsonasync

            if (loginResponse.IsSuccessStatusCode)
            {
                AccessTokenResponse? accessToken = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

                if (accessToken is not null) 
                {
                    await _tokenProvider.StoreToken(accessToken);
                    return true; //logged in 
                
                }
            }
            return false; //not logged in
        }   
    }


}
