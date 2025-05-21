using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using DataAccess;
using DataAccess.Repository;
// Corrected BusinessLayer usings
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Middleware;
using Serilog;
using Serilog.Events;
using WebAPI.Hubs; // Assuming NotificationHub is in WebAPI.Hubs
using WebAPI.Services; // For SignalRNotifier
using AspNetCoreRateLimit; // Added for AspNetCoreRateLimit

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // Optional: Write to a file
                .WriteTo.File("logs/webapi-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                .CreateBootstrapLogger(); // Use CreateBootstrapLogger for early logging

            try
            {
                Log.Information("Starting WebAPI host");

                var builder = WebApplication.CreateBuilder(args);
                var configuration = builder.Configuration;

                // Use Serilog for ASP.NET Core logging
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                // Configure Swagger with JWT support
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Mathematics Consultation Platform API",
                        Version = "v1",
                        Description = "API for the Mathematics Consultation Platform",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Name = "Support Team",
                            Email = "support@mathconsult.com"
                        }
                    });

                    // Add JWT Authentication support in Swagger
                    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

                // Add memory cache
                builder.Services.AddMemoryCache();

                // For response caching on endpoints
                builder.Services.AddResponseCaching();

                builder.Services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                // Register AutoMapper
                // Ensure BusinessLayer.Mapping.MappingProfile exists and is public
                // Or specify the assembly where your profiles are located.
                // For example, if MappingProfile is in BusinessLayer:
                builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(assembly => assembly.GetName().Name == "BusinessLayer"));


                // JWT Authentication
                var jwtKey = configuration["JWT:SecretKey"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("JWT Secret Key is not configured.");
                }

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero // Reduce clock skew to minimum to make token expire exactly at token expiration time
                    };

                    // Configure JWT events (including SignalR)
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Allow JWT authentication for SignalR hub connections
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

                // Corrected service registrations
                builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
                builder.Services.AddScoped<IConsultationService, ConsultationService>();
                // Assuming IUserService and UserService exist in BusinessLayer.Interfaces/Implementations
                // If not, these lines will cause errors and need to be adjusted or the types created.
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<ITokenService, TokenService>();
                builder.Services.AddScoped<IUserFactory, UserFactory>(); // Register UserFactory

                // Add authorization handlers
                builder.Services.AddSingleton<IAuthorizationHandler, WebAPI.Authorization.RoleRequirementHandler>();
                builder.Services.AddSingleton<IAuthorizationHandler, WebAPI.Authorization.PermissionHandler>();
                builder.Services.AddSingleton<IAuthorizationHandler, WebAPI.Authorization.MaterialOwnerHandler>();
                builder.Services.AddSingleton<IAuthorizationHandler, WebAPI.Authorization.ConsultationOwnerHandler>();
                builder.Services.AddSingleton<IAuthorizationHandler, WebAPI.Authorization.ReviewOwnerHandler>();

                // Add authorization policies
                builder.Services.AddAuthorization(options =>
                {
                    // Role-based policies
                    options.AddPolicy("RequireAdministratorRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Administrator")));

                    options.AddPolicy("RequireTeacherRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Teacher")));

                    options.AddPolicy("RequireStudentRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Student")));

                    options.AddPolicy("RequireTeacherOrAdminRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Teacher", "Administrator")));

                    options.AddPolicy("RequireStudentOrTeacherRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Student", "Teacher")));

                    // Permission-based policies
                    options.AddPolicy("CanViewConsultations", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewConsultations)));

                    options.AddPolicy("CanCreateConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.CreateConsultation)));

                    options.AddPolicy("CanEditConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditConsultation)));

                    options.AddPolicy("CanDeleteConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteConsultation)));

                    options.AddPolicy("CanViewMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewMaterials)));

                    options.AddPolicy("CanUploadMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.UploadMaterials)));

                    options.AddPolicy("CanEditMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditMaterials)));

                    options.AddPolicy("CanDeleteMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteMaterials)));

                    options.AddPolicy("CanViewReviews", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewReviews)));

                    options.AddPolicy("CanCreateReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.CreateReview)));

                    options.AddPolicy("CanEditReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditReview)));

                    options.AddPolicy("CanDeleteReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteReview)));

                    options.AddPolicy("CanManageUsers", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ManageUsers)));

                    options.AddPolicy("CanViewStatistics", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewStatistics)));

                    // Resource ownership policies are combined with permission policies in controller methods
                });

                // Add authorization handlers
                builder.Services.AddScoped<IAuthorizationHandler, WebAPI.Authorization.RoleRequirementHandler>();
                builder.Services.AddScoped<IAuthorizationHandler, WebAPI.Authorization.PermissionHandler>();
                builder.Services.AddScoped<IAuthorizationHandler, WebAPI.Authorization.MaterialOwnerHandler>();
                builder.Services.AddScoped<IAuthorizationHandler, WebAPI.Authorization.ConsultationOwnerHandler>();
                builder.Services.AddScoped<IAuthorizationHandler, WebAPI.Authorization.ReviewOwnerHandler>();

                // Add authorization policies
                builder.Services.AddAuthorization(options =>
                {
                    // Role-based policies
                    options.AddPolicy("RequireAdministratorRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Administrator")));

                    options.AddPolicy("RequireTeacherRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Teacher")));

                    options.AddPolicy("RequireStudentRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Student")));

                    options.AddPolicy("RequireTeacherOrAdminRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Teacher", "Administrator")));

                    options.AddPolicy("RequireStudentOrTeacherRole", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.RoleRequirement("Student", "Teacher")));

                    // Permission-based policies
                    options.AddPolicy("CanViewConsultations", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewConsultations)));

                    options.AddPolicy("CanCreateConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.CreateConsultation)));

                    options.AddPolicy("CanEditConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditConsultation)));

                    options.AddPolicy("CanDeleteConsultation", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteConsultation)));

                    options.AddPolicy("CanViewMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewMaterials)));

                    options.AddPolicy("CanUploadMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.UploadMaterials)));

                    options.AddPolicy("CanEditMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditMaterials)));

                    options.AddPolicy("CanDeleteMaterials", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteMaterials)));

                    options.AddPolicy("CanViewReviews", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewReviews)));

                    options.AddPolicy("CanCreateReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.CreateReview)));

                    options.AddPolicy("CanEditReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.EditReview)));

                    options.AddPolicy("CanDeleteReview", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.DeleteReview)));

                    options.AddPolicy("CanManageUsers", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ManageUsers)));

                    options.AddPolicy("CanViewStatistics", policy =>
                        policy.AddRequirements(new WebAPI.Authorization.PermissionRequirement(WebAPI.Authorization.PermissionType.ViewStatistics)));

                    // Resource ownership policies are combined with permission policies in controller methods
                });

                // Register statistics service with strategy pattern
                builder.Services.AddScoped<IStatisticsService, StatisticsService>();
                builder.Services.AddScoped<BusinessLayer.Patterns.Strategy.IRatingStrategy, BusinessLayer.Patterns.Strategy.SimpleAverageStrategy>();
                builder.Services.AddScoped<BusinessLayer.Patterns.Strategy.IRatingStrategy, BusinessLayer.Patterns.Strategy.WeightedAverageStrategy>();
                builder.Services.AddScoped<BusinessLayer.Patterns.Strategy.IRatingStrategy, BusinessLayer.Patterns.Strategy.RecentBiasStrategy>();

                // Register email service using adapter pattern
                builder.Services.AddSingleton<BusinessLayer.Patterns.Adapter.EmailServiceFactory>();
                builder.Services.AddScoped(provider =>
                {
                    var factory = provider.GetRequiredService<BusinessLayer.Patterns.Adapter.EmailServiceFactory>();
                    return factory.CreateEmailService();
                });

                // Register NotificationService and its dependencies (like IHubContext for SignalR later)
                // If NotificationHub is in WebAPI, BusinessLayer cannot directly reference it.
                // Solution: Define an interface IRealtimeNotifier in BusinessLayer, implement in WebAPI,
                // and inject IRealtimeNotifier into NotificationService.
                builder.Services.AddScoped<IRealtimeNotifier, SignalRNotifier>(); // Register the notifier implementation
                builder.Services.AddScoped<INotificationService, NotificationService>();

                // Add memory cache services
                builder.Services.AddMemoryCache(); // Ensure MemoryCache is registered

                // Register Repository implementations
                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));

                // Register async test services for demonstration of repository pattern
                builder.Services.AddScoped<IAsyncTestService, AsyncTestService>();
                builder.Services.AddScoped<IAsyncReviewService, AsyncReviewService>();

                // CORS Configuration
                var allowedOrigins = configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ??
                    new[] { "https://localhost:7063", "http://localhost:5006", "http://localhost:5173" };

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowBlazorClient", policyBuilder =>
                        policyBuilder.WithOrigins(allowedOrigins)
                                   .AllowAnyHeader()
                                   .AllowAnyMethod()
                                   .AllowCredentials()); // Added AllowCredentials for SignalR if needed
                });

                // Configure IP rate limiting
                builder.Services.AddMemoryCache();
                builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
                builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
                builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
                builder.Services.AddInMemoryRateLimiting(); // This helper registers the necessary services for AspNetCoreRateLimit

                // In WebAPI/Program.cs
                // Add this in the service configuration section
                builder.Services.AddSignalR(options =>
                {
                    // Keep connections alive with periodic pings
                    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
                    // Client timeout - should be longer than KeepAliveInterval
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                    // Enable detailed error messages for easier debugging
                    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
                });

                // Make sure to map the Hub after adding services
                // In the Configure section:

                var app = builder.Build();

                app.MapHub<NotificationHub>("/notificationHub");
                // Configure the HTTP request pipeline.
                app.UseSerilogRequestLogging(); // Add Serilog request logging

                // Use response caching middleware
                app.UseResponseCaching();

                app.UseIpRateLimiting();

                app.UseMiddleware<ErrorHandlingMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                // Uncomment to enable HTTPS redirection for production
                // app.UseHttpsRedirection();

                app.UseCors("AllowBlazorClient"); // Apply CORS policy

                app.UseAuthentication(); // Ensure this is before MapHubs
                app.UseAuthorization();  // Ensure this is before MapHubs

                // Map controllers for the API endpoints
                app.MapControllers();

                // Map the SignalR hub endpoint
                app.MapHub<NotificationHub>("/notificationHub");

                // Apply database migrations and seed data
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var dbContext = services.GetRequiredService<MyDbContext>();
                        await DataAccess.Seeding.DatabaseSeeder.SeedDataAsync(dbContext);
                        Log.Information("Database seeded successfully");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "An error occurred while seeding the database");
                    }
                }

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
