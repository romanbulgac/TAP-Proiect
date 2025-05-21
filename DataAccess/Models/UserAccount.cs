using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    /// <summary>
    /// Represents a user's account credentials in the system.
    /// </summary>
    public class UserAccount : AuditEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Changed to have a public setter

        [Required]
        public Guid UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; } = string.Empty; // Can be email or a chosen username

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        // Added for refresh token support
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Add any other account-specific properties if needed, e.g.,
        // public bool IsLockedOut { get; set; }
        // public DateTime? LockoutEndDateUtc { get; set; }
        // public int AccessFailedCount { get; set; }
    }
}