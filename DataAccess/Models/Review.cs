using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public sealed class Review
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [Required]
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;

    [Required]
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
}