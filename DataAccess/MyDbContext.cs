using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MyDbContext : DbContext
    {
        // dotnet ef migrations add <migration-name>
        // dotnet ef database update

        private readonly string _windowsConnectionString = @"Server=.\SQLExpress;Database=TAP_DB_Project;Trusted_Connection=True;TrustServerCertificate=true";

        public DbSet<TestModel> TestModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_windowsConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
