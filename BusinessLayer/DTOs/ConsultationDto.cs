using System;
using System.Collections.Generic;

namespace BusinessLayer.DTOs
{
    public class ConsultationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ConsultationLink { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public decimal Price { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        
        public Guid? StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        
        public List<MaterialDto> Materials { get; set; } = new List<MaterialDto>();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
