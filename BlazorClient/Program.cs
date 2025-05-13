using BlazorClient;
using BlazorClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization; // Required for AuthenticationStateProvider
using System.Net.Http; // Required for IHttpClientFactory
using Microsoft.Extensions.DependencyInjection; // Required for AddHttpClient and other DI extensions


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after"); // Standard for Blazor 8

// Register the custom DelegatingHandler
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Configure HttpClient with base address pointing to WebAPI and the handler
builder.Services.AddHttpClient("WebAPI", client =>
    client.BaseAddress = new Uri("https://localhost:7234")) // IMPORTANT: Update with your WebAPI URL
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

// Make the configured HttpClient available for injection where no specific name is given
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));

// Register Authentication Services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorizationCore();

// Register other client-side services
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
// Register MaterialService and ReviewService similarly

await builder.Build().RunAsync();
