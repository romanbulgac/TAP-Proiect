using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DataAccess
{
    public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
    {
        public MyDbContext CreateDbContext(string[] args)
        {
            // Get the configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // If running from DataAccess project, try to get connection string from WebAPI project
            if (string.IsNullOrEmpty(connectionString))
            {
                // Try from WebAPI folder
                var webApiConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "WebAPI", "appsettings.json");
                if (File.Exists(webApiConfigPath))
                {
                    configuration = new ConfigurationBuilder()
                        .AddJsonFile(webApiConfigPath)
                        .Build();
                    connectionString = configuration.GetConnectionString("DefaultConnection");
                }
            }
            
            // Use hardcoded connection string if still not found
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = @"Server=localhost;Database=MathConsultationDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            }
            
            optionsBuilder.UseSqlServer(connectionString);
            
            return new MyDbContext(optionsBuilder.Options);
        }
    }
}
