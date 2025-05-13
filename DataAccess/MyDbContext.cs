using BCrypt.Net;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
// Potentially: using DataAccess.Models.ValueObjects; // If Email VO is in this namespace

namespace DataAccess
{
    public class MyDbContext : DbContext
    {
        // dotnet ef migrations add <migration-name>
        // dotnet ef database update
        // private readonly string _windowsConnectionString = @"Server=localhost,1433;Database=tema3;User Id=SA;Password=YourPassword123;TrustServerCertificate=True;";
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
        public DbSet<UserAccount> UserAccounts { get; set; }
        // public DbSet<User> Users { get; set; } // Not strictly needed for TPH if querying via derived types


        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection string should be passed via constructor from Program.cs
            if (!optionsBuilder.IsConfigured)
            {
                // Fallback for design-time tools if needed, but prefer DI configuration
                // optionsBuilder.UseSqlServer("your_design_time_connection_string_here_if_absolutely_necessary");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User - UserAccount one-to-one
            builder.Entity<User>()
                .HasOne(u => u.UserAccount)
                .WithOne(ua => ua.User)
                .HasForeignKey<UserAccount>(ua => ua.UserId);

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
                .HasOne(f => f.Teacher) // Student has one Teacher
                .WithOne(c => c.Student) // Teacher has one Student
                .HasForeignKey<Teacher>(c => c.StudentId) // The FK is on Teacher table (Teacher.StudentId refers to a Student)
                .IsRequired(false); // A teacher might not be assigned to a specific student, or a student might not have an assigned teacher directly this way.

            builder.Entity<Teacher>()
                .HasMany(f => f.Consultations)
                .WithOne(c => c.Teacher)
                .HasForeignKey(f => f.TeacherId);
            builder.Entity<ConsultationStudent>()
                .HasKey(cs => new { cs.ConsultationId, cs.StudentId });
            builder.Entity<ConsultationStudent>()
                .HasOne(cs => cs.Consultation)
                .WithMany(c => c.StudentLinks)
                .HasForeignKey(cs => cs.ConsultationId);

            // Review Configurations
            builder.Entity<Review>()
                .HasOne(r => r.Consultation)
                .WithMany(c => c.Reviews) // Assumes Consultation.cs has ICollection<Review> Reviews
                .HasForeignKey(r => r.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade); // Or Restrict, depending on desired behavior

            builder.Entity<Review>()
                .HasOne(r => r.Student) // Assumes Review.Student is the reviewer
                .WithMany(s => s.ReviewsWritten) // Assumes Student.cs has ICollection<Review> ReviewsWritten
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting student if they have reviews, or Cascade

            // Notification Configurations
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications) // Assumes User.cs has ICollection<Notification> Notifications
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Subscription Configurations
            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions) // Assumes User.cs has ICollection<Subscription> Subscriptions
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .HasOne(s => s.SubscribedToTeacher)
                .WithMany() // A teacher can have many subscribers, but Subscription doesn't have a collection nav prop back to Teacher's subscriptions.
                .HasForeignKey(s => s.SubscribedToTeacherId)
                .IsRequired(false) // A subscription might not be to a teacher.
                .OnDelete(DeleteBehavior.Restrict);


            // Index for User Email (Unique)
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            // Configure TPH for User
            builder.Entity<User>()
                .ToTable("Users") // Explicitly set table name for base type
                .HasDiscriminator<string>("Role") // Discriminator column
                .HasValue<Student>("Student")
                .HasValue<Teacher>("Teacher")
                .HasValue<Admin>("Admin");


            // Index for Consultation ScheduledAt
            builder.Entity<Consultation>()
                .HasIndex(c => c.ScheduledAt);

            // Soft Delete Global Query Filters (Example for User)
            // builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted); // Assuming IsDeleted is on AuditEntity
            // Repeat for all entities inheriting AuditEntity and needing soft delete

            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Example: Admin User
            var adminRoleId = Guid.NewGuid(); // Or predefined if you have a Roles table
            var adminId = Guid.NewGuid();
            var adminEmail = "admin@example.com";
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPa$$w0rd");

            builder.Entity<Admin>().HasData(new Admin
            {
                Id = adminId,
                Name = "Admin",
                Surname = "User",
                Email = adminEmail, 
                Role = "Admin", // Discriminator value
                IsActive = true,
                Department = "IT",
                AccessLevel = "Full",
                CreatedAt = DateTime.UtcNow, // From AuditEntity
                UpdatedAt = DateTime.UtcNow  // From AuditEntity
            });

            builder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = Guid.NewGuid(), // Ensure Id is Guid
                UserId = adminId,
                UserName = adminEmail, 
                PasswordHash = adminPasswordHash,
                CreatedAt = DateTime.UtcNow, // From AuditEntity
                UpdatedAt = DateTime.UtcNow  // From AuditEntity
            });

            // Seed Teachers
            var teacherId1 = Guid.NewGuid();
            var teacher1Email = "teacher1@example.com";
            var teacher1PasswordHash = BCrypt.Net.BCrypt.HashPassword("TeacherPa$$w0rd1");
            builder.Entity<Teacher>().HasData(new Teacher { 
                Id = teacherId1, Name = "Alice", Surname = "Smith", Email = teacher1Email, Role = "Teacher", IsActive = true, Subject = "Mathematics", Department = "STEM",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            });
            builder.Entity<UserAccount>().HasData(new UserAccount { 
                Id = Guid.NewGuid(), UserId = teacherId1, UserName = teacher1Email, PasswordHash = teacher1PasswordHash,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow 
            });

            // Seed Students
            var studentId1 = Guid.NewGuid();
            var student1Email = "student1@example.com";
            var student1PasswordHash = BCrypt.Net.BCrypt.HashPassword("StudentPa$$w0rd1");
            builder.Entity<Student>().HasData(new Student { 
                Id = studentId1, Name = "Bob", Surname = "Johnson", Email = student1Email, Role = "Student", IsActive = true, Major = "Computer Science",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            });
            builder.Entity<UserAccount>().HasData(new UserAccount { 
                Id = Guid.NewGuid(), UserId = studentId1, UserName = student1Email, PasswordHash = student1PasswordHash,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            });
            
            // ... Seed other necessary data (Consultations, Materials, etc.)
        }
    }
}
