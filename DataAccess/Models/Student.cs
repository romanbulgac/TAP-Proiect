using System;
using System.Collections.Generic;

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
    private readonly HashSet<Review> _reviewsWritten = new(); // Added for reviews written by the student
    public Guid? TeacherId { get; set; } = null; // Renamed from 'TeachersId'
    public Teacher? Teacher { get; set; } = null!; public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten;
public ICollection<ConsultationStudent> ConsultationLinks { get; set; } = new List<ConsultationStudent>();    
}
