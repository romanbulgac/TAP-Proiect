using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    /// <summary>
    /// Provides shared audit fields for all entities.
    /// </summary>
    public abstract class AuditEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Optional: Add a property for soft delete
        // public bool IsDeleted { get; set; } = false;
    }
}
