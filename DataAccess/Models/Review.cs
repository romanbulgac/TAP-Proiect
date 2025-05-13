using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Review : AuditEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ConsultationId { get; set; }
        [ForeignKey("ConsultationId")]
        public Consultation Consultation { get; set; } = null!;

        [Required]
        public Guid StudentId { get; set; } // Assuming Students write reviews
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; } // e.g., 1 to 5 stars

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}