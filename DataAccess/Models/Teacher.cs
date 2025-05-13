using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public sealed class Teacher : User
{
    public string Subject { get; set; } = string.Empty;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public int YearsOfExperience { get; set; } = 0;
    private readonly HashSet<Consultation> _consultations = new();
    public string Department { get; set; } = string.Empty;
    public IReadOnlyCollection<Consultation> Consultations => _consultations;
    public Student Student { get; set; } = null!;
    public Guid? StudentId { get; set; } = null;
}
