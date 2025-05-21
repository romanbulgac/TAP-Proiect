using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAsyncReviewService
    {
        Task<ReviewDto> CreateReviewAsync(ReviewDto dto);
        Task<IEnumerable<ReviewDto>> GetReviewsForConsultationAsync(Guid consultationId);
        Task<ReviewDto> GetReviewByIdAsync(Guid reviewId);
        Task UpdateReviewAsync(Guid reviewId, ReviewDto dto);
        Task DeleteReviewAsync(Guid reviewId);
        Task<IEnumerable<ReviewDto>> GetReviewsByStudentAsync(Guid studentId);
    }
}
