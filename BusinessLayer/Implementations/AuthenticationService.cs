using BusinessLayer.Interfaces;
using BusinessLayer.DTOs; // Assuming LoginRequestDto, LoginResponseDto are here
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net; // Add this line

namespace BusinessLayer.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        // No longer needs IUserFactory directly for user creation part of Register

        public AuthenticationService(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var userAccount = await _context.UserAccounts
                                    .Include(ua => ua.User) // Include User to get Role
                                    .FirstOrDefaultAsync(ua => ua.User != null && ua.User.Email == email);

            if (userAccount == null || userAccount.User == null)
            {
                return null; // User not found
            }

            if (!BCrypt.Net.BCrypt.Verify(password, userAccount.PasswordHash))
            {
                return null; // Invalid password
            }

            // Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
            var key = Encoding.ASCII.GetBytes(keyString);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userAccount.User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userAccount.User.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(ClaimTypes.Role, userAccount.User.Role)
                // Add other claims as needed
            };
            
            // Add name claim if available
            if (!string.IsNullOrEmpty(userAccount.User.Name) && !string.IsNullOrEmpty(userAccount.User.Surname))
            {
                claims.Add(new Claim(ClaimTypes.Name, $"{userAccount.User.Name} {userAccount.User.Surname}"));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Register method is removed from here and moved to UserService
        // CreateUser factory method is removed from here, belongs to UserFactory
    }
}
