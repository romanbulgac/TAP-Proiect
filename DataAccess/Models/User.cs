using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

/// <summary>
/// Abstract base class for all system users (TPH).
/// </summary>
[Table("Users")]
public abstract class User : AuditEntity
{
    private readonly HashSet<Notification> _notifications = new();
    private readonly HashSet<Subscription> _subscriptions = new();
    
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required, MaxLength(150), EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? ProfilePicture { get; set; } = null;
    public string? PhoneNumber { get; set; } = null;
    public string? Address { get; set; } = null;
    public string? City { get; set; } = null;
    public string? State { get; set; } = null;
    public string? Country { get; set; } = null;
    public string? ZipCode { get; set; } = null;
    public IReadOnlyCollection<Notification> Notifications => _notifications;
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions;

    // Navigation property to UserAccount
    public virtual UserAccount? UserAccount { get; set; }


    protected User()
    {
        // Constructor logic if needed
    }

}
