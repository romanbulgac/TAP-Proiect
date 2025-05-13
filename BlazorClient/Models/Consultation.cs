namespace BlazorClient.Models
{
    public class Consultation
    {
        public Guid Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        // Add other relevant properties
    }
}
