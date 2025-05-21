using BusinessLayer.DTOs;
namespace BlazorClient.Models
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
        public string Error { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }
}
