using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Consultation
    {

        public Guid Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string? Description { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCompleted { get; set; }
        public required Teacher Teacher { get; set; }
        public Guid TeacherId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public string ConsultationStatus { get { return GetStatusDisplay(); } set { } }
        public Type ConsultationType { get; internal set; }
        public string ConsultationLink { get; internal set; } = string.Empty;
        public List<Material> Materials { get;  set; } = new List<Material>();
        public List<ConsultationStudent> StudentLinks { get;  set; } = new List<ConsultationStudent>();
        public List<Review> Reviews { get;  set; } = new List<Review>();

        public string GetStatusDisplay() =>
            IsCancelled ? "Cancelled" :
            IsCompleted ? "Completed" :
            ScheduledAt < DateTime.Now ? "Past" : "Upcoming";
           
        public enum Type
        {
            Group,
            Individual
        }
    }
}