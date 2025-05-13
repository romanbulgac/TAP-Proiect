using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public sealed class Material : AuditEntity
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string ResourceUri { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string FileType { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public Guid? ConsultationId { get; set; }
    public Consultation? Consultation { get; set; }
}