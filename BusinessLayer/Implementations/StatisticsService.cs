using BusinessLayer.Interfaces;
using DataAccess;
using DataAccess.Models;

namespace BusinessLayer.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly MyDbContext _context;
        public StatisticsService(MyDbContext context) => _context = context;

        public double CalculateAverageTeacherRating(Guid teacherId, string strategyName = "simple")
        {
            var reviews = _context.Reviews
                .Where(r => r.Consultation.TeacherId == teacherId)
                .ToList();
            if (!reviews.Any()) return 0.0;

            return strategyName == "weighted"
                ? reviews.Sum(r => r.Rating * 1.5) / (reviews.Count * 1.5)
                : reviews.Average(r => r.Rating);
        }

        public int CountCompletedConsultations()
        {
            return _context.Consultations.Count(c => c.ConsultationStatus == Consultation.Status.Completed);
        }
    }
}
