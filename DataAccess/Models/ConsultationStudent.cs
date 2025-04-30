using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public sealed class ConsultationStudent
{
    public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public bool Attended { get; set; }
}