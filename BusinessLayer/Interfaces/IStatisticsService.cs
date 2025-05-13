namespace BusinessLayer.Interfaces
{
    public interface IStatisticsService
    {
        double CalculateAverageTeacherRating(Guid teacherId, string strategyName = "simple");
        int CountCompletedConsultations();
        // ...other stats: popular materials, etc.
    }
}
