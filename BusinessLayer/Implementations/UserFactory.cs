using BusinessLayer.Interfaces;
using DataAccess.Models;
using BusinessLayer.DTOs; // For RegisterRequestDto

namespace BusinessLayer.Implementations
{
    public class UserFactory : IUserFactory
    {
        // DTO now contains all necessary info except hashed password
        public User CreateUser(RegisterRequestDto dto, string hashedPassword /* This implies UserAccount is handled elsewhere or User needs PasswordHash directly */)
        {
            // This factory should ideally only create the User object.
            // The UserAccount object with the hashed password should be created and linked in the UserService.
            // For simplicity here, if User model itself had a PasswordHash (not recommended for clean TPH), it would be set here.
            // Let's assume the factory just creates the User profile part.

            User user = dto.Role.ToLower() switch
            {
                "admin" => new Admin { Name = dto.Name, Surname = dto.Surname, Email = dto.Email, Role = "Admin" /* , other Admin props */ },
                "teacher" => new Teacher { Name = dto.Name, Surname = dto.Surname, Email = dto.Email, Role = "Teacher" /* , other Teacher props */ },
                _ => new Student { Name = dto.Name, Surname = dto.Surname, Email = dto.Email, Role = "Student" /* , other Student props */ },
            };
            // User.PasswordHash = hashedPassword; // This line would be here IF User model stored the hash directly.
                                                // But it's better in UserAccount.
            return user;
        }
    }
}
