using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IStatisticsService
    {
        Task<double> CalculateAverageTeacherRatingAsync(Guid teacherId, string strategyName = "simple");
        Task<int> CountCompletedConsultationsAsync();
        IDictionary<string, string> GetAvailableRatingStrategies();
        Task<IDictionary<string, double>> GetTeacherPopularityStatsAsync();
    }
}
