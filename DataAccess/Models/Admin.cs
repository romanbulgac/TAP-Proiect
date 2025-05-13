using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public sealed class Admin : User
{
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public int YearsOfExperience { get; set; } = 0;
    public string Permissions { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string AccessLevel { get; set; } = string.Empty;
    public string Responsibilities { get; set; } = string.Empty;
}
