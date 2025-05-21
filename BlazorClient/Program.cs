using BlazorClient;
using BlazorClient.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization; // Required for AuthenticationStateProvider
using Microsoft.JSInterop; // Required for IJSRuntime
using System.Net.Http; // Required for IHttpClientFactory
using Microsoft.Extensions.DependencyInjection; // Required for AddHttpClient and other DI extensions
using Blazored.LocalStorage; // Required for AddBlazoredLocalStorage


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after"); // Standard for Blazor 8

// Configure HttpClient for API calls that don't require authorization (login, register)
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5217"); 
    client.DefaultRequestHeaders.Add("User-Agent", "BlazorClient");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configure HttpClient for Web API usage (e.g., NotificationService)
builder.Services.AddHttpClient("WebAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5217");
});

// Register browser storage - needs to be before services that depend on it
builder.Services.AddBlazoredLocalStorage();

// Register TokenService before auth services
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Authentication State Provider with its own HttpClient (no auth handler)
// This breaks the circular dependency
builder.Services.AddHttpClient("AuthStateClient", client => 
{
    client.BaseAddress = new Uri("http://localhost:5217");
    client.DefaultRequestHeaders.Add("User-Agent", "BlazorClient");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<CustomAuthenticationStateProvider>(sp => 
    new CustomAuthenticationStateProvider(
        sp.GetRequiredService<ITokenService>(),
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthStateClient"),
        sp.GetRequiredService<ILoggerFactory>().CreateLogger<CustomAuthenticationStateProvider>()
    )
);

builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<CustomAuthenticationStateProvider>()
);

builder.Services.AddAuthorizationCore();

// Register the custom DelegatingHandler for authorized requests
builder.Services.AddTransient<AuthorizationMessageHandler>();

// Configure HttpClient for API calls that require authorization (with handler)
// This must come after Auth State Provider registration to avoid circular dependency
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5217");
    client.DefaultRequestHeaders.Add("User-Agent", "BlazorClient");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// Make the "ApiClient" available for injection where a default HttpClient is expected
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register other client-side services
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<INotificationService>(provider => new NotificationService(
    provider.GetRequiredService<IHttpClientFactory>(),
    provider.GetRequiredService<IJSRuntime>(),
    provider.GetRequiredService<ILoggerFactory>().CreateLogger<NotificationService>(),
    provider.GetRequiredService<AuthenticationStateProvider>()
)); 
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<AppState>();

try
{
    var host = builder.Build();
    
    // Create logger to debug application startup
    var logger = host.Services.GetRequiredService<ILoggerFactory>()
        .CreateLogger<Program>();
    
    logger.LogInformation("Starting BlazorClient application");
    
    // Hook up unhandled exception handler
    AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
    {
        logger.LogError(error.ExceptionObject as Exception, "Unhandled application exception");
    };
    
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    // This will help identify startup issues in the browser console
    await Task.Delay(1000); // Give some time for the message to be logged
}
