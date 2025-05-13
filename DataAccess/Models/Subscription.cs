using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public class Subscription : AuditEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; } // The user who is subscribed
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    // Example: Subscribing to a Teacher's new materials or consultations
    public Guid? SubscribedToTeacherId { get; set; }
    [ForeignKey("SubscribedToTeacherId")]
    public Teacher? SubscribedToTeacher { get; set; }

    // Example: Subscribing to a specific type of content or entity
    // public Guid? TargetEntityId { get; set; } // ID of the entity being subscribed to (e.g., a course, a forum thread)
    // [MaxLength(100)]
    // public string? TargetEntityType { get; set; } // Type of the entity (e.g., "Course", "Teacher")

    [Required, MaxLength(100)]
    public string SubscriptionType { get; set; } = string.Empty; // e.g., "NewMaterialsFromTeacher", "ConsultationUpdates"

    public DateTime SubscriptionDate { get; set; } = DateTime.UtcNow;
}
