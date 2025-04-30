using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class MyDbContext : DbContext
    {
        // dotnet ef migrations add <migration-name>
        // dotnet ef database update
        private readonly string _windowsConnectionString = @"Server=localhost,1433;Database=tema3;User Id=SA;Password=YourPassword123;TrustServerCertificate=True;";
        // private readonly string _windowsConnectionString = @"Server=.\SQLExpress;Database=TAP_DB_Project;Trusted_Connection=True;TrustServerCertificate=true";

        public DbSet<TestModel> TestModels { get; set; }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ConsultationStudent> ConsultationStudents { get; set; } 
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_windowsConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // one to many
            builder.Entity<Consultation>()
                .HasOne(f => f.Teacher)
                .WithMany(c => c.Consultations)
                .HasForeignKey(f => f.TeacherId);
            builder.Entity<Consultation>()
                .HasMany(f => f.StudentLinks)
                .WithOne(c => c.Consultation)
                .HasForeignKey(f => f.ConsultationId);
            builder.Entity<Consultation>()
                .HasMany(f => f.Materials)
                .WithOne(c => c.Consultation)
                .HasForeignKey(f => f.ConsultationId);
            builder.Entity<Consultation>()
                .HasMany(f => f.Reviews)
                .WithOne(c => c.Consultation)
                .HasForeignKey(f => f.ConsultationId);

            builder.Entity<Student>()
                .HasOne(f => f.Teacher)
                .WithOne(c => c.Student)
                .HasForeignKey<Teacher>(c => c.StudentId);


            
        }
    }
}
