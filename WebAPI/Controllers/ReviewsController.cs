using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAsyncReviewService _asyncReviewService;
        private readonly IAuthorizationService _authorizationService;

        public ReviewsController(
            IReviewService reviewService, 
            IAsyncReviewService asyncReviewService,
            IAuthorizationService authorizationService)
        {
            _reviewService = reviewService;
            _asyncReviewService = asyncReviewService;
            _authorizationService = authorizationService;
        }

        // Legacy synchronous endpoint
        [HttpPost("sync")]
        [Authorize(Policy = "CanCreateReview")]
        public IActionResult CreateReviewSync([FromBody] ReviewDto dto)
        {
            _reviewService.CreateReview(dto);
            return Ok();
        }

        // Legacy synchronous endpoint
        [HttpGet("sync/consultation/{consultationId}")]
        [Authorize(Policy = "CanViewReviews")]
        public IActionResult GetReviewsForConsultationSync(Guid consultationId)
        {
            var reviews = _reviewService.GetReviewsForConsultation(consultationId);
            return Ok(reviews);
        }

        // Async endpoints
        [HttpPost]
        [Authorize(Policy = "CanCreateReview")]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] ReviewDto dto)
        {
            // Validate that the current user is the student or admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrator") && userId != dto.StudentId.ToString())
            {
                return Forbid();
            }

            try
            {
                var createdReview = await _asyncReviewService.CreateReviewAsync(dto);
                return CreatedAtAction(nameof(GetReview), new { id = createdReview.Id }, createdReview);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("consultation/{consultationId}")]
        [Authorize(Policy = "CanViewReviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForConsultation(Guid consultationId)
        {
            var reviews = await _asyncReviewService.GetReviewsForConsultationAsync(consultationId);
            return Ok(reviews);
        }
        
        [HttpGet("{id}")]
        [Authorize(Policy = "CanViewReviews")]
        public async Task<ActionResult<ReviewDto>> GetReview(Guid id)
        {
            try
            {
                var review = await _asyncReviewService.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Policy = "CanViewReviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByStudent(Guid studentId)
        {
            // Use resource-based authorization
            if (!User.IsInRole("Administrator"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != studentId.ToString())
                {
                    return Forbid();
                }
            }

            var reviews = await _asyncReviewService.GetReviewsByStudentAsync(studentId);
            return Ok(reviews);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanEditReview")]
        public async Task<IActionResult> UpdateReview(Guid id, ReviewDto reviewDto)
        {
            try
            {
                // Get the review to check resource-based authorization
                var review = await _asyncReviewService.GetReviewByIdAsync(id);
                
                // Use authorization service for resource-based authorization
                var authorizationResult = await _authorizationService.AuthorizeAsync(
                    User, 
                    review, 
                    new WebAPI.Authorization.ResourceOwnerRequirement("Administrator"));
                
                if (!authorizationResult.Succeeded)
                {
                    return Forbid();
                }

                await _asyncReviewService.UpdateReviewAsync(id, reviewDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteReview")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            try
            {
                // Get the review to check resource-based authorization
                var review = await _asyncReviewService.GetReviewByIdAsync(id);
                
                // Use authorization service for resource-based authorization
                var authorizationResult = await _authorizationService.AuthorizeAsync(
                    User, 
                    review, 
                    new WebAPI.Authorization.ResourceOwnerRequirement("Administrator"));
                
                if (!authorizationResult.Succeeded)
                {
                    return Forbid();
                }

                await _asyncReviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
