using DataAccess.Models;
using BusinessLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BusinessLayer.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GenerateJwtToken(User user, IEnumerable<Claim>? additionalClaims = null)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? 
                throw new InvalidOperationException("JWT SecretKey is not configured."));
            var issuer = jwtSettings["Issuer"] ?? 
                throw new InvalidOperationException("JWT Issuer is not configured.");
            var audience = jwtSettings["Audience"] ?? 
                throw new InvalidOperationException("JWT Audience is not configured.");
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token identifier
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
                new Claim(ClaimTypes.Role, user.Role)
            };
            
            // Add any additional claims if provided
            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["DurationInHours"] ?? "24")),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = jwtSettings["SecretKey"] ?? 
                throw new InvalidOperationException("JWT SecretKey is not configured.");
                
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                // Allow to validate the expiration date
                ValidateLifetime = false
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                                                      StringComparison.InvariantCultureIgnoreCase))
                    return null;
                
                return principal;
            }
            catch
            {
                return null;
            }
        }
        
        public string GenerateRefreshToken()
        {
            // Creating a cryptographically secure random token
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
        public int GetRefreshTokenValidityInDays()
        {
            var jwtSettings = _configuration.GetSection("JWT");
            return Convert.ToInt32(jwtSettings["RefreshTokenValidityInDays"] ?? "7");
        }
    }
}
