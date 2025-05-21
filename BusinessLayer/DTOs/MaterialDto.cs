using System;

namespace BusinessLayer.DTOs
{
    public class MaterialDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResourceUri { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid TeacherId { get; set; } // Added
        public Guid? ConsultationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}