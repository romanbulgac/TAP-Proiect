using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public sealed class Consultation
{
    public enum Status
    {
        Scheduled,
        Completed,
        Cancelled
    }
    public enum Type
    {
        Individual,
        Group
    }
    private readonly HashSet<ConsultationStudent> _studentLinks = new();
    private readonly HashSet<Material> _materials = new();
    private readonly HashSet<Review> _reviews = new();

    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Topic { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime ScheduledAt { get; set; }

    public int DurationMinutes { get; set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public Status ConsultationStatus { get; set; } = Status.Scheduled;
    public Type ConsultationType { get; set; } = Type.Individual;

    public string ConsultationLink { get; set; } = string.Empty;
    public string? Notes { get; set; } = null;

    [Required]
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    [InverseProperty(nameof(ConsultationStudent.Consultation))]
    public IReadOnlyCollection<ConsultationStudent> StudentLinks => _studentLinks;

    /// <summary>Материалы, прикреплённые к сессии.</summary>
    public IReadOnlyCollection<Material> Materials => _materials;

    /// <summary>Отзывы студентов о данной сессии.</summary>
    public IReadOnlyCollection<Review> Reviews => _reviews;
}