using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Models;

namespace DataAccess.Models;

public sealed class Notification
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime SentAt { get; private set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User user { get; set; } = null!;
}