using System;

namespace BusinessLayer.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public Guid ConsultationId { get; set; }
        public string? ConsultationTitle { get; set; }
        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
