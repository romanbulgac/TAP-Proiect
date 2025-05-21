// filepath: /Users/romanbulgac/Documents/University/Semestru VI/TAP/Proiect/BusinessLayer/Implementations/AuthenticationService.cs
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess.Models; // Required for User model (returned by IUserService)
using System;
using System.Threading.Tasks;
using BCrypt.Net; // Added for password hashing
using System.Security.Claims; // Added for ClaimTypes
using System.Linq; // Added for LINQ operations like FirstOrDefault

namespace BusinessLayer.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<AuthenticationResultDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequestDto.Email);
            if (user == null)
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "User not found." };
            }

            var isPasswordValid = await _userService.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isPasswordValid)
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "Invalid password." };
            }

            var token = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            // Store refresh token in database
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_tokenService.GetRefreshTokenValidityInDays());
            await _userService.SetRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);

            return new AuthenticationResultDto
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto 
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = user.Role
                }
            };
        }

        public async Task<AuthenticationResultDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userService.GetUserByEmailAsync(registerRequestDto.Email);
            if (existingUser != null)
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "User with this email already exists." };
            }
            
            try
            {
                // UserService now handles password hashing
                var createdUser = await _userService.CreateUserAsync(registerRequestDto);
                if (createdUser == null)
                {
                     return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "User registration failed." };
                }

                var token = _tokenService.GenerateJwtToken(createdUser);
                var refreshToken = _tokenService.GenerateRefreshToken();
                
                // Store refresh token in database
                var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_tokenService.GetRefreshTokenValidityInDays());
                await _userService.SetRefreshTokenAsync(createdUser.Id, refreshToken, refreshTokenExpiryTime);

                return new AuthenticationResultDto
                {
                    IsSuccess = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = createdUser.Id,
                        Email = createdUser.Email,
                        Name = createdUser.Name,
                        Surname = createdUser.Surname,
                        Role = createdUser.Role
                    }
                };
            }
            catch (Exception ex)
            {
                // TODO: Replace with proper logging (e.g., Serilog)
                Console.WriteLine($"Error during registration: {ex.Message} StackTrace: {ex.StackTrace}"); 
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "An error occurred during registration." };
            }
        }
        
        public async Task<AuthenticationResultDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequestDto.Token);
            if (principal == null)
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "Invalid access token" };
            }

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "Invalid token claim" };
            }

            // Validate refresh token
            var (isValid, user) = await _userService.ValidateRefreshTokenAsync(tokenRequestDto.RefreshToken);
            if (!isValid || user == null || user.Id != userId)
            {
                return new AuthenticationResultDto { IsSuccess = false, ErrorMessage = "Invalid refresh token" };
            }

            // Generate new tokens
            var newToken = _tokenService.GenerateJwtToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            
            // Update refresh token in database
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_tokenService.GetRefreshTokenValidityInDays());
            await _userService.SetRefreshTokenAsync(user.Id, newRefreshToken, refreshTokenExpiryTime);

            return new AuthenticationResultDto
            {
                IsSuccess = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = user.Role
                }
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            // Validate refresh token and get the associated user
            var (isValid, user) = await _userService.ValidateRefreshTokenAsync(refreshToken);
            if (!isValid || user == null)
            {
                return false;
            }

            // Invalidate token by setting it to empty string instead of null
            await _userService.SetRefreshTokenAsync(user.Id, string.Empty, DateTime.UtcNow);
            return true;
        }
    }
}
