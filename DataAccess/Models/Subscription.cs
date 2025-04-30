using System;

namespace DataAccess.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the subscription

    public Guid StudentId { get; set; } // Identifier for the student who owns the subscription

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow; // Timestamp when the subscription was created
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow; 
    public DateTime ExpiresAt { get; set; } // Timestamp when the subscription expires
    public DateTime? LastRenewedAt { get; set; } // Timestamp when the subscription was last renewed

    public bool IsActive { get; set; } = true; // Indicates whether the subscription is currently active
}
