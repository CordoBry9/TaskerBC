using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskerBC;
using TaskerBC.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<TaskerAPIMessageHandler>();
builder.Services.AddHttpClient("TaskerApiClient", client => client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]!))
    .AddHttpMessageHandler<TaskerAPIMessageHandler>();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<TaskerItemService>();



await builder.Build().RunAsync();

