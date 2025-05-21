// filepath: /Users/romanbulgac/Documents/University/Semestru VI/TAP/Proiect/BusinessLayer/Implementations/UserService.cs
using DataAccess;
using DataAccess.Models;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using BCrypt.Net;
using Microsoft.Extensions.Caching.Memory; // Added for IMemoryCache
using AutoMapper; // Added for IMapper
using System.Collections.Generic; // Added for IEnumerable

namespace BusinessLayer.Implementations
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IUserFactory _userFactory;
        private readonly IMemoryCache _cache; // Added
        private readonly IMapper _mapper; // Added
        private const string AllTeachersCacheKey = "AllTeachersList"; // Added

        public UserService(MyDbContext context, IUserFactory userFactory, IMemoryCache cache, IMapper mapper) // Updated constructor
        {
            _context = context;
            _userFactory = userFactory;
            _cache = cache; // Added
            _mapper = mapper; // Added
        }

        public async Task<IEnumerable<UserDto>> GetAllTeachersAsync() // Added method
        {
            if (!_cache.TryGetValue(AllTeachersCacheKey, out IEnumerable<UserDto>? teachers) || teachers == null)
            {
                var teacherEntities = await _context.Teachers
                    .Where(t => t.IsActive)
                    .AsNoTracking() // Good practice for read-only queries
                    .ToListAsync();
                teachers = _mapper.Map<IEnumerable<UserDto>>(teacherEntities);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15)) // Example: Cache for 15 mins
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));  // Or max 1 hour

                _cache.Set(AllTeachersCacheKey, teachers, cacheEntryOptions);
            }
            return teachers!;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Check all user types
            var student = await _context.Students
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (student != null) return student;

            var teacher = await _context.Teachers
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (teacher != null) return teacher;

            var admin = await _context.Admins
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Email == email);
            return admin;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            // Check all user types
            var student = await _context.Students
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (student != null) return student;

            var teacher = await _context.Teachers
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (teacher != null) return teacher;

            var admin = await _context.Admins
                .Include(u => u.UserAccount)
                .FirstOrDefaultAsync(u => u.Id == id);
            return admin;
        }

        public async Task<User> CreateUserAsync(RegisterRequestDto registerRequestDto)
        {
            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password);
            
            // Create the appropriate user type based on role using the factory
            var user = _userFactory.CreateUser(registerRequestDto, passwordHash);
            
            // Create and link the UserAccount
            var userAccount = new UserAccount
            {
                UserId = user.Id,
                UserName = registerRequestDto.Email,
                PasswordHash = passwordHash,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            user.UserAccount = userAccount;
            
            // Add to the appropriate DbSet based on type
            if (user is Student student)
            {
                _context.Students.Add(student);
            }
            else if (user is Teacher teacher)
            {
                _context.Teachers.Add(teacher);
            }
            else if (user is Admin admin)
            {
                _context.Admins.Add(admin);
            }
            
            await _context.SaveChangesAsync();

            if (user.Role == "Teacher") // Invalidate cache if a new teacher is added
            {
                _cache.Remove(AllTeachersCacheKey);
            }

            return user;
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (user.UserAccount == null)
            {
                return Task.FromResult(false);
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.UserAccount.PasswordHash);
            return Task.FromResult(isValid);
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false; // This is for logical deactivation, not soft delete.
                                   // If soft delete is intended, this method should also set IsDeleted = true.
            user.UpdatedAt = DateTime.UtcNow;
            
            // If DeactivateUserAsync should also soft delete:
            // if (user is ISoftDelete softDeletableUser)
            // {
            //     softDeletableUser.IsDeleted = true;
            //     softDeletableUser.DeletedAt = DateTime.UtcNow;
            // }

            await _context.SaveChangesAsync();

            if (user.Role == "Teacher") // Invalidate cache if a teacher is deactivated/modified
            {
                _cache.Remove(AllTeachersCacheKey);
            }
            return true;
        }

        public async Task<UserAccount?> GetUserAccountByUserIdAsync(Guid userId)
        {
            return await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.UserId == userId);
        }

        public async Task SetRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime)
        {
            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.UserId == userId);

            if (userAccount != null)
            {
                userAccount.RefreshToken = refreshToken;
                userAccount.RefreshTokenExpiryTime = expiryTime;
                userAccount.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(bool IsValid, User? User)> ValidateRefreshTokenAsync(string refreshToken)
        {
            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(ua => ua.RefreshToken == refreshToken && 
                                          ua.RefreshTokenExpiryTime > DateTime.UtcNow);

            if (userAccount == null)
            {
                return (false, null);
            }

            var user = await GetUserByIdAsync(userAccount.UserId);
            return (true, user);
        }

        // Add this method for cache invalidation when a teacher is updated
        public async Task InvalidateTeacherCacheAsync()
        {
            _cache.Remove(AllTeachersCacheKey);
            await Task.CompletedTask;
        }
    }
}
