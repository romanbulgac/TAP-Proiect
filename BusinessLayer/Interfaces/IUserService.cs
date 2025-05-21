using DataAccess.Models;
using BusinessLayer.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic; // Added for IEnumerable

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllTeachersAsync(); // Added
        Task InvalidateTeacherCacheAsync();
        Task<User> CreateUserAsync(RegisterRequestDto registerRequestDto);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> DeactivateUserAsync(Guid userId);
        
        // Refresh token operations
        Task<UserAccount?> GetUserAccountByUserIdAsync(Guid userId);
        Task SetRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime);
        Task<(bool IsValid, User? User)> ValidateRefreshTokenAsync(string refreshToken);
    }
}
