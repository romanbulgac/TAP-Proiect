namespace BusinessLayer.DTOs
{
    public class ConsultationDto
    {
        public Guid Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string ConsultationLink { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        // ...other fields if needed (Status, etc.)...
    }
}
