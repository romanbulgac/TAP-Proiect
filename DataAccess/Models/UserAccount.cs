using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public sealed class UserAccount
{
    [Key, ForeignKey(nameof(User))]
    public Guid Id { get; private set; }

    [Required, MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Навигация к пользователю.</summary>
    public User User { get; set; } = null!;
}