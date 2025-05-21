using System;

namespace BusinessLayer.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string Type { get; set; } = "System";
        public Guid? ConsultationId { get; set; }
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
