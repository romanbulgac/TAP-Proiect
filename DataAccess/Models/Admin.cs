using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public sealed class Admin : User
{
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    public int YearsOfExperience { get; set; } = 0;

    [MaxLength(500)]
    public string Responsibilities { get; set; } = string.Empty;

    [MaxLength(100)]
    public string AccessLevel { get; set; } = "Basic"; // e.g., Basic, Full

    [MaxLength(500)]
    public string Permissions { get; set; } = string.Empty; // Could be a JSON string or comma-separated
}
