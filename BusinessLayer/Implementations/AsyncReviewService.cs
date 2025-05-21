using BusinessLayer.Base;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace BusinessLayer.Implementations
{
    public class AsyncReviewService : BaseAsyncService<Review, ReviewDto>, IAsyncReviewService
    {
        private readonly MyDbContext _context;
        
        public AsyncReviewService(IAsyncRepository<Review> repository, IMapper mapper, MyDbContext context) 
            : base(repository, mapper)
        {
            _context = context;
        }

        public async Task<ReviewDto> CreateReviewAsync(ReviewDto dto)
        {
            // Map the DTO to an entity
            var review = Mapper.Map<Review>(dto);
            
            // Set the created date if not already set
            if (review.CreatedAt == default)
            {
                review.CreatedAt = DateTime.UtcNow;
            }
            
            // Add to the repository
            await Repository.AddAsync(review);
            await Repository.SaveChangesAsync();
            
            // Get the updated entity with related data
            var createdReview = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Consultation)
                .FirstOrDefaultAsync(r => r.Id == review.Id);
                
            if (createdReview == null)
                throw new Exception("Failed to retrieve created review");
                
            // Map back to DTO with enhanced data
            var resultDto = Mapper.Map<ReviewDto>(createdReview);
            resultDto.ConsultationTitle = createdReview.Consultation?.Topic;
            resultDto.StudentName = $"{createdReview.Student?.Name} {createdReview.Student?.Surname}".Trim();
            
            return resultDto;
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Consultation)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
                
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
                
            var dto = Mapper.Map<ReviewDto>(review);
            
            // Enhanced with related data
            dto.ConsultationTitle = review.Consultation?.Topic;
            dto.StudentName = $"{review.Student?.Name} {review.Student?.Surname}".Trim();
            
            return dto;
        }
        
        public async Task<IEnumerable<ReviewDto>> GetReviewsForConsultationAsync(Guid consultationId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ConsultationId == consultationId)
                .Include(r => r.Student)
                .Include(r => r.Consultation)
                .AsNoTracking()
                .ToListAsync();
                
            var dtos = Mapper.Map<IEnumerable<ReviewDto>>(reviews).ToList();
            
            // Enhance each DTO with related entity data
            foreach (var (dto, review) in dtos.Zip(reviews, (d, r) => (d, r)))
            {
                dto.ConsultationTitle = review.Consultation?.Topic;
                dto.StudentName = $"{review.Student?.Name} {review.Student?.Surname}".Trim();
            }
            
            return dtos.OrderByDescending(r => r.Rating);
        }
        
        public async Task<IEnumerable<ReviewDto>> GetReviewsByStudentAsync(Guid studentId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Consultation)
                .AsNoTracking()
                .ToListAsync();
                
            var dtos = Mapper.Map<IEnumerable<ReviewDto>>(reviews).ToList();
            
            // Enhance DTOs with consultation data
            foreach (var (dto, review) in dtos.Zip(reviews, (d, r) => (d, r)))
            {
                dto.ConsultationTitle = review.Consultation?.Topic;
            }
            
            return dtos.OrderByDescending(r => r.CreatedAt);
        }

        public async Task UpdateReviewAsync(Guid reviewId, ReviewDto dto)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            
            // Update properties
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;
            
            // Update via repository
            await Repository.UpdateAsync(review);
            await Repository.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            var review = await Repository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {reviewId} not found");
            
            // Use the base service method which handles soft delete if applicable
            await base.DeleteAsync(reviewId);
        }

        public override async Task<ReviewDto> GetByIdAsync(Guid id)
        {
            return await GetReviewByIdAsync(id);
        }
    }
}
