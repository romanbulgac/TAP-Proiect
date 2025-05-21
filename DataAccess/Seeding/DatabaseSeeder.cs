using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace DataAccess.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(MyDbContext context)
        {
            Console.WriteLine("[DatabaseSeeder] Checking if seeding is required.");

            
                Console.WriteLine("[DatabaseSeeder] No user accounts found. Proceeding with seeding.");
                // Add admin user
                var admin = new Admin
                {
                    Id = Guid.NewGuid(),
                    Name = "System",
                    Surname = "Administrator",
                    Email = "admin@mathconsult.com",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var adminAccount = new UserAccount
                {
                    Id = Guid.NewGuid(),
                    UserId = admin.Id,
                    UserName = admin.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    CreatedAt = DateTime.UtcNow,
                    User = admin
                };
                
                // Add sample teachers
                var teacher1Id = Guid.NewGuid();
                var teacher2Id = Guid.NewGuid();
                
                var teachers = new List<Teacher>
                {
                    new Teacher
                    {
                        Id = teacher1Id,
                        Name = "Emma",
                        Surname = "Johnson",
                        Email = "emma.johnson@mathconsult.com",
                        Role = "Teacher",
                        CreatedAt = DateTime.UtcNow,
                        Subject = "Calculus",
                        Department = "Mathematics",
                        HireDate = DateTime.UtcNow.AddYears(-3),
                        YearsOfExperience = 5
                    },
                    new Teacher
                    {
                        Id = teacher2Id,
                        Name = "David",
                        Surname = "Smith",
                        Email = "david.smith@mathconsult.com",
                        Role = "Teacher",
                        CreatedAt = DateTime.UtcNow,
                        Subject = "Algebra",
                        Department = "Mathematics",
                        HireDate = DateTime.UtcNow.AddYears(-5),
                        YearsOfExperience = 8
                    }
                };
                
                var teacherAccounts = new List<UserAccount>
                {
                    new UserAccount
                    {
                        Id = Guid.NewGuid(),
                        UserId = teacher1Id,
                        UserName = "emma.johnson@mathconsult.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                        CreatedAt = DateTime.UtcNow,
                        User = teachers[0]
                    },
                    new UserAccount
                    {
                        Id = Guid.NewGuid(),
                        UserId = teacher2Id,
                        UserName = "david.smith@mathconsult.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"), // Password for David Smith
                        CreatedAt = DateTime.UtcNow,
                        User = teachers[1]
                    }
                };
                
                // Add sample students
                var student1Id = Guid.NewGuid();
                var student2Id = Guid.NewGuid();
                
                var students = new List<Student>
                {
                    new Student
                    {
                        Id = student1Id,
                        Name = "Alex",
                        Surname = "Brown",
                        Email = "alex.brown@mathconsult.com",
                        Role = "Student",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Student
                    {
                        Id = student2Id,
                        Name = "Sophia",
                        Surname = "Wilson",
                        Email = "sophia.wilson@mathconsult.com",
                        Role = "Student",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                
                var studentAccounts = new List<UserAccount>
                {
                    new UserAccount
                    {
                        Id = Guid.NewGuid(),
                        UserId = student1Id,
                        UserName = "alex.brown@mathconsult.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                        CreatedAt = DateTime.UtcNow,
                        User = students[0]
                    },
                    new UserAccount
                    {
                        Id = Guid.NewGuid(),
                        UserId = student2Id,
                        UserName = "sophia.wilson@mathconsult.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                        CreatedAt = DateTime.UtcNow,
                        User = students[1]
                    }
                };
                
                // Add consultations
                var consultations = new List<Consultation>
                {
                    new Consultation
                    {
                        Id = Guid.NewGuid(),
                        Topic = "Introduction to Derivatives",
                        Description = "Basic concepts of derivatives and their applications",
                        Teacher= teachers[0],
                        TeacherId = teachers[0].Id,
                        ScheduledAt = DateTime.UtcNow.AddDays(3), // Future date
                        DurationMinutes = 60,
                        CreatedAt = DateTime.UtcNow,
                        ConsultationStatus = "Scheduled", // Using string status
                        ConsultationType = Consultation.Type.Group, // Using enum type
                        ConsultationLink = "https://meet.google.com/abc-defg-hij"
                    },
                    new Consultation
                    {
                        Id = Guid.NewGuid(),
                        Topic = "Linear Equations Systems",
                        Description = "Solving systems of linear equations using matrices",
                        Teacher = teachers[1],
                        TeacherId = teachers[1].Id,
                        ScheduledAt = DateTime.UtcNow.AddDays(5), // Future date
                        DurationMinutes = 90,
                        CreatedAt = DateTime.UtcNow,
                        ConsultationStatus = "Scheduled", // Using string status
                        ConsultationType = Consultation.Type.Individual, // Using enum type
                        ConsultationLink = "https://meet.google.com/xyz-abcd-efg"
                    },
                    new Consultation // Past consultation for testing
                    {
                        Id = Guid.NewGuid(),
                        Topic = "Past Topic: Limits",
                        Description = "Review of limits",
                        Teacher = teachers[0],
                        TeacherId = teachers[0].Id,
                        ScheduledAt = DateTime.UtcNow.AddDays(-7), // Past date
                        DurationMinutes = 45,
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        ConsultationStatus = "Completed",
                        ConsultationType = Consultation.Type.Group,
                        ConsultationLink = "https://meet.google.com/past-limits"
                    }
                };
                
                // Add materials
                var materials = new List<Material>
                {
                    new Material
                    {
                        Id = Guid.NewGuid(),
                        Title = "Derivatives Cheat Sheet",
                        Description = "Quick reference for common derivatives",
                        ResourceUri = "https://storage.mathconsult.com/materials/derivatives-cheatsheet.pdf",
                        FileType = "PDF",
                        ConsultationId = consultations[0].Id,
                        TeacherId = teachers[0].Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Material
                    {
                        Id = Guid.NewGuid(),
                        Title = "Matrix Operations Guide",
                        Description = "Detailed guide on matrix operations",
                        ResourceUri = "https://storage.mathconsult.com/materials/matrix-operations.pdf",
                        FileType = "PDF",
                        ConsultationId = consultations[1].Id,
                        TeacherId = teachers[1].Id,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                
                // Book students for consultations
                var consultationStudents = new List<ConsultationStudent>
                {
                    new ConsultationStudent
                    {
                        ConsultationId = consultations[0].Id,
                        StudentId = students[0].Id,
                        Attended = false
                    },
                    new ConsultationStudent
                    {
                        ConsultationId = consultations[1].Id,
                        StudentId = students[1].Id,
                        Attended = false
                    },
                    // Student Alex also attended the past consultation
                    new ConsultationStudent
                    {
                        ConsultationId = consultations[2].Id, // Past consultation
                        StudentId = students[0].Id,
                        Attended = true 
                    }
                };
                
                // Add entities to context
                context.Admins.Add(admin);
                context.Teachers.AddRange(teachers);
                context.Students.AddRange(students);
                context.UserAccounts.Add(adminAccount);
                context.UserAccounts.AddRange(teacherAccounts);
                context.UserAccounts.AddRange(studentAccounts);
                context.Consultations.AddRange(consultations);
                context.Materials.AddRange(materials);
                context.ConsultationStudents.AddRange(consultationStudents);
                
                Console.WriteLine("[DatabaseSeeder] All entities prepared. Attempting to save changes to the database.");
                // Save changes
                try
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine("[DatabaseSeeder] Data seeded successfully and saved to the database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DatabaseSeeder] Error saving seeded data to the database: {ex.Message}");
                    // It's important to understand why saving might fail. Re-throw or handle as appropriate.
                    throw; 
                }
            }
        }
    }

