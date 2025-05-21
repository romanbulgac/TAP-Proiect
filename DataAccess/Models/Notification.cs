using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Models;

namespace DataAccess.Models;

public sealed class Notification : AuditEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;

    [MaxLength(50)]
    public string NotificationType { get; set; } = "System";

    public Guid? ConsultationId { get; set; }
    [ForeignKey("ConsultationId")]
    public Consultation? Consultation { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [MaxLength(255)]
    public string? Link { get; set; } // Optional link for the notification
}