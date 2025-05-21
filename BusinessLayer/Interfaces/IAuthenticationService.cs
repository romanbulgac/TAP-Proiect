using BusinessLayer.DTOs;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResultDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<AuthenticationResultDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthenticationResultDto> RefreshTokenAsync(TokenRequestDto tokenRequestDto);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
