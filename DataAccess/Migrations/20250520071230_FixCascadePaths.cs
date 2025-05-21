using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationStudents_Users_StudentId",
                table: "ConsultationStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_StudentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StudentId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("cc9751b2-1830-49bc-ac23-df690f801ff3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a9283451-fb58-4c43-bb70-27d6d5522773"));

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Responsibilities",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Admin_Department",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessLevel",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TestModels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TestModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TestModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TestModels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Department", "Discriminator", "Email", "HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Subject", "Surname", "UpdatedAt", "YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("0aa8296e-00e0-4667-b0c1-f90361e4155d"), null, null, null, new DateTime(2025, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4580), null, "Science", "Teacher", "teacher@example.com", new DateTime(2023, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4580), true, false, new DateTime(2025, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4570), "John", null, null, "Teacher", null, "Mathematics", "Doe", new DateTime(2025, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4580), 5, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessLevel", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Admin_Department", "Discriminator", "Email", "Admin_HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "Permissions", "PhoneNumber", "ProfilePicture", "Responsibilities", "Role", "State", "Surname", "UpdatedAt", "Admin_YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("58f84dc0-85d9-4b5d-853b-c8ae874d25d0"), "Full", null, null, null, new DateTime(2025, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(8890), null, "IT", "Admin", "admin@example.com", new DateTime(2024, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(8890), true, false, new DateTime(2025, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(8890), "Admin", "All", null, null, "System Administration", "Administrator", null, "User", new DateTime(2025, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(8890), 1, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Discriminator", "Email", "EnrollmentDate", "GPA", "IsActive", "IsDeleted", "LastLogin", "Major", "Minor", "Name", "PhoneNumber", "ProfilePicture", "Role", "State", "Status", "Surname", "TeacherId", "UpdatedAt", "YearOfStudy", "ZipCode" },
                values: new object[] { new Guid("6b97626a-d404-4fac-b3b4-732f0b7a0296"), null, null, null, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8370), null, "Student", "student@example.com", new DateTime(2024, 11, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8340), 0.0, true, false, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8340), "Computer Science", "", "Jane", null, null, "Student", null, "Active", "Smith", null, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8370), 1, null });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "ConsultationId", "CreatedAt", "DeletedAt", "Description", "FileType", "IsDeleted", "ResourceUri", "TeacherId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("854723f4-c6b2-4b0f-a59b-e800bc5db133"), null, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8480), null, "Basic concepts of algebra.", "PDF", false, "/materials/algebra_intro.pdf", new Guid("0aa8296e-00e0-4667-b0c1-f90361e4155d"), "Introduction to Algebra", new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8480) },
                    { new Guid("c103ed73-1846-4f79-939b-6d35ef6eea68"), null, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8480), null, "Quick reference for calculus formulas.", "PDF", false, "/materials/calculus_cheatsheet.pdf", new Guid("0aa8296e-00e0-4667-b0c1-f90361e4155d"), "Calculus Cheat Sheet", new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8480) }
                });

            migrationBuilder.InsertData(
                table: "UserAccounts",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserId", "UserName" },
                values: new object[,]
                {
                    { new Guid("241cf4be-d61b-460e-810f-d2c95b8fba27"), new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8450), null, false, "$2a$11$Cdm8pGQ7gdztoFIJ9FXxiOB4lN91asGcqJAwqoaGtOsMeVt21B/9.", null, null, new DateTime(2025, 5, 20, 7, 12, 30, 613, DateTimeKind.Utc).AddTicks(8450), new Guid("6b97626a-d404-4fac-b3b4-732f0b7a0296"), "student@example.com" },
                    { new Guid("522c62f8-3e7d-4257-ac29-cda8c4341b95"), new DateTime(2025, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4660), null, false, "$2a$11$s6MWekM3psCSGFsIEXg3VOw3kxtKKKH3vNr7dj5PSydfbvrvC0bf6", null, null, new DateTime(2025, 5, 20, 7, 12, 30, 499, DateTimeKind.Utc).AddTicks(4660), new Guid("0aa8296e-00e0-4667-b0c1-f90361e4155d"), "teacher@example.com" },
                    { new Guid("767a1f3c-660a-439c-9515-57792a003129"), new DateTime(2025, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(9100), null, false, "$2a$11$LQKtMCB2NHfl6wt41ScbTee6bQukcxvsuSfv2g7bHZbUrGggq9wY6", null, null, new DateTime(2025, 5, 20, 7, 12, 30, 387, DateTimeKind.Utc).AddTicks(9100), new Guid("58f84dc0-85d9-4b5d-853b-c8ae874d25d0"), "admin@example.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeacherId",
                table: "Users",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationStudents_Users_StudentId",
                table: "ConsultationStudents",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_TeacherId",
                table: "Users",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationStudents_Users_StudentId",
                table: "ConsultationStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_TeacherId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeacherId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("854723f4-c6b2-4b0f-a59b-e800bc5db133"));

            migrationBuilder.DeleteData(
                table: "Materials",
                keyColumn: "Id",
                keyValue: new Guid("c103ed73-1846-4f79-939b-6d35ef6eea68"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("241cf4be-d61b-460e-810f-d2c95b8fba27"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("522c62f8-3e7d-4257-ac29-cda8c4341b95"));

            migrationBuilder.DeleteData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: new Guid("767a1f3c-660a-439c-9515-57792a003129"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0aa8296e-00e0-4667-b0c1-f90361e4155d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("58f84dc0-85d9-4b5d-853b-c8ae874d25d0"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6b97626a-d404-4fac-b3b4-732f0b7a0296"));

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TestModels");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TestModels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TestModels");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TestModels");

            migrationBuilder.AlterColumn<string>(
                name: "Responsibilities",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Admin_Department",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessLevel",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessLevel", "Address", "City", "Country", "CreatedAt", "DeletedAt", "Admin_Department", "Discriminator", "Email", "Admin_HireDate", "IsActive", "IsDeleted", "LastLogin", "Name", "Permissions", "PhoneNumber", "ProfilePicture", "Responsibilities", "Role", "State", "Surname", "UpdatedAt", "Admin_YearsOfExperience", "ZipCode" },
                values: new object[] { new Guid("a9283451-fb58-4c43-bb70-27d6d5522773"), "Full", null, null, null, new DateTime(2025, 5, 16, 12, 59, 34, 33, DateTimeKind.Utc).AddTicks(710), null, "IT", "Admin", "admin@example.com", new DateTime(2024, 5, 16, 12, 59, 34, 33, DateTimeKind.Utc).AddTicks(720), true, false, new DateTime(2025, 5, 16, 12, 59, 34, 33, DateTimeKind.Utc).AddTicks(710), "Admin", "All", null, null, "System Administration", "Administrator", null, "User", new DateTime(2025, 5, 16, 12, 59, 34, 33, DateTimeKind.Utc).AddTicks(710), 1, null });

            migrationBuilder.InsertData(
                table: "UserAccounts",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserId", "UserName" },
                values: new object[] { new Guid("cc9751b2-1830-49bc-ac23-df690f801ff3"), new DateTime(2025, 5, 16, 12, 59, 34, 149, DateTimeKind.Utc).AddTicks(1500), null, false, "$2a$11$hNOqJwUzsZwM4il7QdA/LeawHVOdOwY.fYsjOaMzv1mKp99RfihVy", null, null, new DateTime(2025, 5, 16, 12, 59, 34, 149, DateTimeKind.Utc).AddTicks(1500), new Guid("a9283451-fb58-4c43-bb70-27d6d5522773"), "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentId",
                table: "Users",
                column: "StudentId",
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationStudents_Users_StudentId",
                table: "ConsultationStudents",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_StudentId",
                table: "Users",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
