namespace BusinessLayer.Interfaces
{
    public interface IReviewService
    {
        void CreateReview(DTOs.ReviewDto dto);
        IEnumerable<DTOs.ReviewDto> GetReviewsForConsultation(Guid consultationId);
    }
}
