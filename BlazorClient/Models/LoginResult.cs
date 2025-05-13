namespace BlazorClient.Models
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
