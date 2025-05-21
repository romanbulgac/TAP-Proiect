using DataAccess.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace BusinessLayer.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, IEnumerable<Claim>? additionalClaims = null);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        int GetRefreshTokenValidityInDays();
    }
}
