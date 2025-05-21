namespace BusinessLayer.DTOs
{
    public class AuthenticationResultDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? ErrorMessage { get; set; }
        public UserDto? User { get; set; } // Optional: to return some user info
    }
}
