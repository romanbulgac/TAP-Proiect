using BCrypt.Net;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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

            // Apply filters only on root types in TPH hierarchies
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Skip non-root types for TPH inheritance
                if (entityType.BaseType != null)
                    continue;

                // Apply the soft delete filter only for types that implement AuditEntity
                if (typeof(AuditEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Property(parameter, nameof(AuditEntity.IsDeleted)),
                        Expression.Constant(false));
                    var lambda = Expression.Lambda(body, parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

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

            // If Student has one Teacher (advisor)
            builder.Entity<Student>()
                .HasOne(s => s.Teacher)
                .WithMany() // A teacher can advise multiple students, but a student has one advisor.
                            // If a teacher can only advise one student (strict 1-to-1), then WithOne() on Teacher side is needed.
                            // Assuming a teacher can advise multiple students, this becomes a one-to-many from Teacher to Student.
                            // Or, if it's truly one-to-one, Student.TeacherId is the FK.
                .HasForeignKey(s => s.TeacherId)
                .IsRequired(false); // A student might not have an assigned teacher.

            builder.Entity<Teacher>()
                .HasMany(f => f.Consultations)
                .WithOne(c => c.Teacher)
                .HasForeignKey(f => f.TeacherId);
            builder.Entity<ConsultationStudent>()
                .HasKey(cs => new { cs.ConsultationId, cs.StudentId });

            builder.Entity<ConsultationStudent>()
                .HasOne(cs => cs.Consultation)
                .WithMany(c => c.StudentLinks)
                .HasForeignKey(cs => cs.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ConsultationStudent>()
    .HasOne(cs => cs.Student)
    .WithMany(s => s.ConsultationLinks) // Refer to the collection here
    .HasForeignKey(cs => cs.StudentId)
    .OnDelete(DeleteBehavior.NoAction);

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

            // Seed data
            var adminId = Guid.NewGuid();
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!");
            builder.Entity<Admin>().HasData(new Admin
            {
                Id = adminId,
                Name = "Admin",
                Surname = "User",
                Email = "admin@example.com",
                Role = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Department = "IT",
                HireDate = DateTime.UtcNow.AddYears(-1),
                YearsOfExperience = 1,
                Permissions = "All",
                AccessLevel = "Full",
                Responsibilities = "System Administration"
            });
            builder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = Guid.NewGuid(),
                UserId = adminId,
                UserName = "admin@example.com", // Use email as username for consistency
                PasswordHash = adminPasswordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            var teacherId = Guid.NewGuid();
            var teacherPasswordHash = BCrypt.Net.BCrypt.HashPassword("TeacherPassword123!");
            builder.Entity<Teacher>().HasData(new Teacher
            {
                Id = teacherId,
                Name = "John",
                Surname = "Doe",
                Email = "teacher@example.com",
                Role = "Teacher",
                IsActive = true,
                Subject = "Mathematics",
                HireDate = DateTime.UtcNow.AddYears(-2),
                YearsOfExperience = 5,
                Department = "Science",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            builder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = Guid.NewGuid(),
                UserId = teacherId,
                UserName = "teacher@example.com",
                PasswordHash = teacherPasswordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            var studentId = Guid.NewGuid();
            var studentPasswordHash = BCrypt.Net.BCrypt.HashPassword("StudentPassword123!");
            builder.Entity<Student>().HasData(new Student
            {
                Id = studentId,
                Name = "Jane",
                Surname = "Smith",
                Email = "student@example.com",
                Role = "Student",
                IsActive = true,
                EnrollmentDate = DateTime.UtcNow.AddMonths(-6),
                YearOfStudy = 1,
                Major = "Computer Science",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            builder.Entity<UserAccount>().HasData(new UserAccount
            {
                Id = Guid.NewGuid(),
                UserId = studentId,
                UserName = "student@example.com",
                PasswordHash = studentPasswordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            builder.Entity<Material>().HasData(
                new Material {
                    Id = Guid.NewGuid(), // This should now work
                    Title = "Introduction to Algebra",
                    ResourceUri = "/materials/algebra_intro.pdf",
                    FileType = "PDF",
                    Description = "Basic concepts of algebra.",
                    TeacherId = teacherId, // Assuming teacherId is the ID of the seeded teacher
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Material {
                    Id = Guid.NewGuid(), // This should now work
                    Title = "Calculus Cheat Sheet",
                    ResourceUri = "/materials/calculus_cheatsheet.pdf",
                    FileType = "PDF",
                    Description = "Quick reference for calculus formulas.",
                    TeacherId = teacherId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var now = DateTime.UtcNow;
            
            foreach (var entry in ChangeTracker.Entries<AuditEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.IsDeleted = false;
                    entry.Entity.DeletedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    
                    // Check if this is a soft delete operation
                    if (entry.Entity.IsDeleted && !entry.Property(nameof(ISoftDelete.DeletedAt)).IsModified)
                    {
                        entry.Entity.DeletedAt = now;
                    }
                }
            }
        }

        private static System.Linq.Expressions.LambdaExpression ConvertLambdaExpression<TInterface>(System.Linq.Expressions.Expression<Func<TInterface, bool>> expression, Type entityType)
        {
            var newParam = System.Linq.Expressions.Expression.Parameter(entityType);
            var newBody = System.Linq.Expressions.Expression.Invoke(expression, System.Linq.Expressions.Expression.Convert(newParam, typeof(TInterface)));
            return System.Linq.Expressions.Expression.Lambda(newBody, newParam);
        }
    }
}
