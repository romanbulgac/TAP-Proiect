using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public IActionResult CreateReview([FromBody] ReviewDto dto)
        {
            _reviewService.CreateReview(dto);
            return Ok();
        }

        [HttpGet("consultation/{consultationId}")]
        public IActionResult GetReviewsForConsultation(Guid consultationId)
        {
            var reviews = _reviewService.GetReviewsForConsultation(consultationId);
            return Ok(reviews);
        }
    }
}
