using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("35f6c42f-86e7-4c71-85c9-893cc409c3bd"));

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("d8ec1feb-821d-45a2-8996-8cd01fa8ae91"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("4603c4e3-43c3-48ad-8845-dc169dca1610"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("711abbbc-7d14-4838-b305-c92e1e37fcfb"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("8fe645f7-fb09-4340-9310-8ed5c5b79c43"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("030d9f52-7324-4717-ab47-343e33cfe861"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("52ac27c8-ceaa-4ec4-a0a5-1d8c20852aa5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9898dfd0-aab4-4830-a78c-ac94ab0f5170"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessLevel", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Admin_Department", "Discriminator", "Email", "Admin_HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "Permissions", "PhoneNumber", "ProfilePicture", "Responsibilities", "Role", "State", "Surname", "UpdatedAt", "Admin_YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("50736532-e657-4d4e-98ae-26c0c977e54a"), "Full", null, null, null, new DateTime(2025, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4730), null, "IT", "Admin", "admin@example.com", new DateTime(2024, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4730), true, false, new DateTime(2025, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4730), "Admin", "All", null, null, "System Administration", "Administrator", null, "User", new DateTime(2025, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4730), 1, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Discriminator", "Email", "EnrollmentDate", "GPA", "IsActive", "IsDeleted", "LastLogin", "Major", "Minor", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Status", "Surname", "TeacherId", "UpdatedAt", "YearOfStudy", "ZipCode" },
                values: new object[] { new Guid("9bb8703c-e732-4ad5-99ab-dc4fc63d6a43"), null, null, null, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7760), null, "Student", "student@example.com", new DateTime(2024, 11, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7750), 0.0, true, false, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7750), "Computer Science", "", "Jane", null, null, "Student", null, "Active", "Smith", null, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7760), 1, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Department", "Discriminator", "Email", "HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Subject", "Surname", "UpdatedAt", "YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("e3527241-65d0-49bd-98fb-048e248c94b6"), null, null, null, new DateTime(2025, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6690), null, "Science", "Teacher", "teacher@example.com", new DateTime(2023, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6690), true, false, new DateTime(2025, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6690), "John", null, null, "Teacher", null, "Mathematics", "Doe", new DateTime(2025, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6690), 5, null });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "ConsultationId", "CreatedAt", "DeletedAt", "Description", "FileType", "IsDeleted", "ResourceUri", "TeacherId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5fbf3cfd-d292-4f03-9f3d-9d332054b84a"), null, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7830), null, "Basic concepts of algebra.", "PDF", false, "/materials/algebra_intro.pdf", new Guid("e3527241-65d0-49bd-98fb-048e248c94b6"), "Introduction to Algebra", new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7830) },
                    { new Guid("b3e8ae8f-a401-4a49-9c97-ea48b078e3ec"), null, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7830), null, "Quick reference for calculus formulas.", "PDF", false, "/materials/calculus_cheatsheet.pdf", new Guid("e3527241-65d0-49bd-98fb-048e248c94b6"), "Calculus Cheat Sheet", new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7840) }
                });

            migrationBuilder.InsertData(
                table: "UserAccounts",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserId", "UserName" },
                values: new object[,]
                {
                    { new Guid("0d03f101-1e17-4bd3-9c9d-f374ca95a66f"), new DateTime(2025, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4900), null, false, "$2a$11$tnWLt0NwB0z/MEUkqCtd/.tO.R3iDjkwUxP3RSO4r6uOaAKF0b57O", null, null, new DateTime(2025, 5, 21, 16, 45, 23, 524, DateTimeKind.Utc).AddTicks(4900), new Guid("50736532-e657-4d4e-98ae-26c0c977e54a"), "admin@example.com" },
                    { new Guid("b4dea876-e707-4592-9445-e37ef691feb6"), new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7800), null, false, "$2a$11$yRn3rWWiYVZLufi/g1TlDeZBlDakW9Tg.FI1YRVmh6aSLVMI70ODu", null, null, new DateTime(2025, 5, 21, 16, 45, 23, 746, DateTimeKind.Utc).AddTicks(7800), new Guid("9bb8703c-e732-4ad5-99ab-dc4fc63d6a43"), "student@example.com" },
                    { new Guid("e7fa046d-18db-4c76-9f19-414036ed1ce0"), new DateTime(2025, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6770), null, false, "$2a$11$rwIbnIxORKdEcTWjz9WxO.WxtvPnTxPRlDTxj8vrFquH/DrvNZV2G", null, null, new DateTime(2025, 5, 21, 16, 45, 23, 634, DateTimeKind.Utc).AddTicks(6770), new Guid("e3527241-65d0-49bd-98fb-048e248c94b6"), "teacher@example.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("5fbf3cfd-d292-4f03-9f3d-9d332054b84a"));

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("b3e8ae8f-a401-4a49-9c97-ea48b078e3ec"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("0d03f101-1e17-4bd3-9c9d-f374ca95a66f"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("b4dea876-e707-4592-9445-e37ef691feb6"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("e7fa046d-18db-4c76-9f19-414036ed1ce0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50736532-e657-4d4e-98ae-26c0c977e54a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9bb8703c-e732-4ad5-99ab-dc4fc63d6a43"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e3527241-65d0-49bd-98fb-048e248c94b6"));

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
        }
    }
}
