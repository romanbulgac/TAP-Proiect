using System;

namespace DataAccess.Models;

public sealed class Student : User
{
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public double GPA { get; set; } = 0.0;
    public int YearOfStudy { get; set; } = 1;
    public string Major { get; set; } = string.Empty;
    public string Minor { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    private readonly HashSet<ConsultationStudent> _consultationLinks = new();
    public Guid? TeachersId { get; set; } = null;
    public Teacher? Teacher { get; set; } = null!;
    
}
