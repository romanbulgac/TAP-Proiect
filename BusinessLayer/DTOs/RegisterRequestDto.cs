namespace BusinessLayer.DTOs
{
    public class RegisterRequestDto
    {
        public string Role { get; set; } = "Student";
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
