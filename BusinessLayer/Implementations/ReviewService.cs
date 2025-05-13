using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;

namespace BusinessLayer.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly MyDbContext _context;
        public ReviewService(MyDbContext context) => _context = context;

        public void CreateReview(ReviewDto dto)
        {
            var entity = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                ConsultationId = dto.ConsultationId,
                StudentId = dto.StudentId
            };
            _context.Reviews.Add(entity);
            _context.SaveChanges();
        }

        public IEnumerable<ReviewDto> GetReviewsForConsultation(Guid consultationId)
        {
            return _context.Reviews
                .Where(r => r.ConsultationId == consultationId)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    StudentId = r.StudentId
                })
                .OrderByDescending(r => r.Rating)
                .ToList();
        }
    }
}
