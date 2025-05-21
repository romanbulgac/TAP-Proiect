using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Admin_Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Admin_HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Admin_YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    Responsibilities = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Permissions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GPA = table.Column<double>(type: "float", nullable: true),
                    YearOfStudy = table.Column<int>(type: "int", nullable: true),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsultationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsultationType = table.Column<int>(type: "int", nullable: false),
                    ConsultationLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consultations_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribedToTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubscriptionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_SubscribedToTeacherId",
                        column: x => x.SubscribedToTeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationStudents",
                columns: table => new
                {
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attended = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationStudents", x => new { x.ConsultationId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ConsultationStudents_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationStudents_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResourceUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Materials_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Discriminator", "Email", "EnrollmentDate", "GPA", "IsActive", "IsDeleted", "LastLogin", "Major", "Minor", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Status", "Surname", "TeacherId", "UpdatedAt", "YearOfStudy", "ZipCode" },
                values: new object[] { new Guid("030d9f52-7324-4717-ab47-343e33cfe861"), null, null, null, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9570), null, "Student", "student@example.com", new DateTime(2024, 11, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9560), 0.0, true, false, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9560), "Computer Science", "", "Jane", null, null, "Student", null, "Active", "Smith", null, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9570), 1, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessLevel", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Admin_Department", "Discriminator", "Email", "Admin_HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "Permissions", "PhoneNumber", "ProfilePicture", "Responsibilities", "Role", "State", "Surname", "UpdatedAt", "Admin_YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("52ac27c8-ceaa-4ec4-a0a5-1d8c20852aa5"), "Full", null, null, null, new DateTime(2025, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1460), null, "IT", "Admin", "admin@example.com", new DateTime(2024, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1460), true, false, new DateTime(2025, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1450), "Admin", "All", null, null, "System Administration", "Administrator", null, "User", new DateTime(2025, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1460), 1, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Department", "Discriminator", "Email", "HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Subject", "Surname", "UpdatedAt", "YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("9898dfd0-aab4-4830-a78c-ac94ab0f5170"), null, null, null, new DateTime(2025, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1930), null, "Science", "Teacher", "teacher@example.com", new DateTime(2023, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1920), true, false, new DateTime(2025, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1920), "John", null, null, "Teacher", null, "Mathematics", "Doe", new DateTime(2025, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1930), 5, null });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "ConsultationId", "CreatedAt", "DeletedAt", "Description", "FileType", "IsDeleted", "ResourceUri", "TeacherId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("35f6c42f-86e7-4c71-85c9-893cc409c3bd"), null, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9650), null, "Basic concepts of algebra.", "PDF", false, "/materials/algebra_intro.pdf", new Guid("9898dfd0-aab4-4830-a78c-ac94ab0f5170"), "Introduction to Algebra", new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9650) },
                    { new Guid("d8ec1feb-821d-45a2-8996-8cd01fa8ae91"), null, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9650), null, "Quick reference for calculus formulas.", "PDF", false, "/materials/calculus_cheatsheet.pdf", new Guid("9898dfd0-aab4-4830-a78c-ac94ab0f5170"), "Calculus Cheat Sheet", new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9660) }
                });

            migrationBuilder.InsertData(
                table: "UserAccounts",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserId", "UserName" },
                values: new object[,]
                {
                    { new Guid("4603c4e3-43c3-48ad-8845-dc169dca1610"), new DateTime(2025, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1670), null, false, "$2a$11$M2WVzn1AAZXo.YFgc2WRVuozEqJ8NeLKLhaWcbNmI/Ervss6CHs8q", null, null, new DateTime(2025, 5, 21, 16, 39, 46, 365, DateTimeKind.Utc).AddTicks(1670), new Guid("52ac27c8-ceaa-4ec4-a0a5-1d8c20852aa5"), "admin@example.com" },
                    { new Guid("711abbbc-7d14-4838-b305-c92e1e37fcfb"), new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9620), null, false, "$2a$11$I99fbks9dySyzTlkWKFAluaTVRYtTc.epyPpD8anrlfegJqtdR7xC", null, null, new DateTime(2025, 5, 21, 16, 39, 46, 588, DateTimeKind.Utc).AddTicks(9620), new Guid("030d9f52-7324-4717-ab47-343e33cfe861"), "student@example.com" },
                    { new Guid("8fe645f7-fb09-4340-9310-8ed5c5b79c43"), new DateTime(2025, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1980), null, false, "$2a$11$MPmQrM61NF3mcJ92RsaL4e3absHUI7ALFpcVbR6f/ZNSQG1QzDy0e", null, null, new DateTime(2025, 5, 21, 16, 39, 46, 475, DateTimeKind.Utc).AddTicks(1980), new Guid("9898dfd0-aab4-4830-a78c-ac94ab0f5170"), "teacher@example.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_TeacherId",
                table: "Consultations",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationStudents_StudentId",
                table: "ConsultationStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ConsultationId",
                table: "Materials",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_TeacherId",
                table: "Materials",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ConsultationId",
                table: "Notifications",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ConsultationId",
                table: "Reviews",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StudentId",
                table: "Reviews",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscribedToTeacherId",
                table: "Subscriptions",
                column: "SubscribedToTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserId",
                table: "UserAccounts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeacherId",
                table: "Users",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationStudents");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "TestModels");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
