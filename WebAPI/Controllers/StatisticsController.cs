using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Secure all endpoints by default
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(IStatisticsService statisticsService, ILogger<StatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        [HttpGet("teacher-rating/{teacherId}")]
        [AllowAnonymous] // Example: Publicly visible statistic
        public async Task<ActionResult<double>> GetTeacherRating(Guid teacherId, [FromQuery] string strategy = "simple")
        {
            try
            {
                var rating = await _statisticsService.CalculateAverageTeacherRatingAsync(teacherId, strategy);
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating teacher rating");
                return StatusCode(500, "An error occurred while calculating the teacher rating.");
            }
        }

        [HttpGet("completed-consultations")]
        [Authorize(Roles = "Administrator,Teacher")] // Example: Only for admin/teacher
        public async Task<ActionResult<int>> GetCompletedConsultationsCount()
        {
            try
            {
                var count = await _statisticsService.CountCompletedConsultationsAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting completed consultations");
                return StatusCode(500, "An error occurred while counting completed consultations.");
            }
        }

        [HttpGet("rating-strategies")]
        [AllowAnonymous] // Publicly visible
        public ActionResult<IDictionary<string, string>> GetRatingStrategies()
        {
            try
            {
                var strategies = _statisticsService.GetAvailableRatingStrategies();
                return Ok(strategies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rating strategies");
                return StatusCode(500, "An error occurred while retrieving rating strategies.");
            }
        }

        [HttpGet("teacher-popularity")]
        [Authorize(Roles = "Administrator")] 
        public async Task<ActionResult<IDictionary<string, double>>> GetTeacherPopularity()
        {
            try
            {
                var popularity = await _statisticsService.GetTeacherPopularityStatsAsync();
                return Ok(popularity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher popularity stats");
                return StatusCode(500, "An error occurred while retrieving teacher popularity statistics.");
            }
        }
    }
}
