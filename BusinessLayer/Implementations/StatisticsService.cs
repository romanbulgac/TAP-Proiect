using BusinessLayer.Interfaces;
using BusinessLayer.Patterns.Strategy;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async

namespace BusinessLayer.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly MyDbContext _context;
        private readonly IEnumerable<IRatingStrategy> _ratingStrategies; // Injected collection of strategies

        public StatisticsService(MyDbContext context, IEnumerable<IRatingStrategy> ratingStrategies)
        {
            _context = context;
            _ratingStrategies = ratingStrategies;
        }

        public async Task<double> CalculateAverageTeacherRatingAsync(Guid teacherId, string strategyName = "simple")
        {
            var strategy = _ratingStrategies.FirstOrDefault(s => s.Name.Equals(strategyName, StringComparison.OrdinalIgnoreCase))
                           ?? _ratingStrategies.FirstOrDefault(s => s.Name.Equals("simple", StringComparison.OrdinalIgnoreCase)) // Fallback
                           ?? throw new ArgumentException($"Rating strategy '{strategyName}' not found.");

            var reviews = await _context.Reviews
                .Where(r => r.Consultation.TeacherId == teacherId)
                .Include(r => r.Consultation) // Ensure consultation is loaded for TeacherId access
                .AsNoTracking()
                .ToListAsync();

            return strategy.CalculateAverageRating(reviews);
        }

        public async Task<int> CountCompletedConsultationsAsync()
        {
            return await _context.Consultations
                .CountAsync(c => c.ConsultationStatus == "Completed");
        }

        public IDictionary<string, string> GetAvailableRatingStrategies()
        {
            return _ratingStrategies.ToDictionary(s => s.Name, s => s.Description);
        }

        public async Task<IDictionary<string, double>> GetTeacherPopularityStatsAsync()
        {
            // Example: Popularity based on number of booked consultations
            var stats = await _context.Consultations
                .Where(c => c.StudentLinks.Any()) // Consider only booked consultations
                .GroupBy(c => c.Teacher.Email) // Group by teacher's email or name
                .Select(g => new { TeacherEmail = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.TeacherEmail, x => (double)x.Count);
            return stats;
        }
    }
}
