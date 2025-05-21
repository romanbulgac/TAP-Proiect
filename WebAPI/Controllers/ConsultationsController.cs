using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BusinessLayer.Attributes;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All actions require authentication by default
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationService _consultationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<ConsultationsController> _logger;

        public ConsultationsController(
            IConsultationService consultationService,
            IAuthorizationService authorizationService,
            ILogger<ConsultationsController> logger)
        {
            _consultationService = consultationService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateConsultation")]
        public async Task<IActionResult> CreateConsultation([FromBody] ConsultationDto consultationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            // Ensure the TeacherId is set correctly for the current user if they're a teacher
            if (User.IsInRole("Teacher"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userId, out Guid teacherId))
                {
                    consultationDto.TeacherId = teacherId;
                }
            }
            
            var consultationId = await _consultationService.CreateConsultationAsync(consultationDto);
            return CreatedAtAction(nameof(GetConsultationById), new { id = consultationId }, consultationDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanViewConsultations")]
        public async Task<IActionResult> GetConsultationById(Guid id)
        {
            try
            {
                var consultation = await _consultationService.GetConsultationByIdAsync(id);
                if (consultation == null) return NotFound();
                
                // Map to anonymous object to ensure clean serialization
                var result = new 
                {
                    id = consultation.Id.ToString(),
                    title = consultation.Title ?? string.Empty,
                    topic = consultation.Topic ?? string.Empty,
                    description = consultation.Description ?? string.Empty,
                    scheduledAt = consultation.ScheduledAt.ToString("o"), // ISO 8601 format
                    durationMinutes = consultation.DurationMinutes,
                    status = consultation.Status ?? string.Empty,
                    location = consultation.Location ?? string.Empty,
                    isOnline = consultation.IsOnline,
                    teacherId = consultation.TeacherId.ToString(),
                    teacherName = consultation.TeacherName ?? string.Empty
                };
                
                var jsonText = System.Text.Json.JsonSerializer.Serialize(result);
                return Content(jsonText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consultation by ID {Id}: {Message}", id, ex.Message);
                return StatusCode(500, "An error occurred while retrieving the consultation.");
            }
        }

        [HttpGet]
        [Authorize(Policy = "CanViewConsultations")]
        [ResponseCache(Duration = 60)] // Cache for 1 minute
        public async Task<IActionResult> GetAllConsultations()
        {
            try
            {
                _logger.LogInformation("Request received for all consultations");
                
                // Using anonymous types to avoid any serialization issues
                var result = new List<object>();
                
                var allConsultations = await _consultationService.GetAllConsultationsAsync();
                _logger.LogInformation($"Retrieved {allConsultations?.Count() ?? 0} consultations from service");
                
                if (allConsultations != null)
                {
                    // Map to anonymous objects to ensure clean serialization
                    var consultations = allConsultations
                        .Where(c => c != null)
                        .Select(c => new 
                        {
                            id = c.Id.ToString(),
                            title = c.Title ?? string.Empty,
                            topic = c.Topic ?? string.Empty,
                            description = c.Description ?? string.Empty,
                            scheduledAt = c.ScheduledAt.ToString("o"), // ISO 8601 format
                            durationMinutes = c.DurationMinutes,
                            status = c.Status ?? string.Empty,
                            location = c.Location ?? string.Empty,
                            isOnline = c.IsOnline,
                            teacherId = c.TeacherId.ToString(),
                            teacherName = c.TeacherName ?? string.Empty
                        })
                        .ToList();
                        
                    // Convert to generic objects to avoid type serialization issues
                    foreach (var consultation in consultations)
                    {
                        result.Add(consultation);
                    }
                    
                    _logger.LogInformation($"Returning {result.Count} consultations");
                }
                
                // Manually build a JSON response
                var jsonText = System.Text.Json.JsonSerializer.Serialize(result);
                return Content(jsonText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all consultations: {Message}", ex.Message);
                _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                return Content("[]", "application/json");
            }
        }
        
        [HttpPost("{consultationId}/book/{studentId}")]
        [Authorize(Policy = "RequireStudentOrTeacherRole")]
        public async Task<IActionResult> BookConsultation(Guid consultationId, Guid studentId)
        {
            // If user is a student, they can only book for themselves
            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != studentId.ToString())
                {
                    return Forbid();
                }
            }
            
            await _consultationService.BookConsultationAsync(consultationId, studentId);
            return Ok();
        }

        [HttpPut("{consultationId}/cancel")]
        [Authorize(Policy = "RequireStudentOrTeacherRole")]
        public async Task<IActionResult> CancelConsultation(Guid consultationId)
        {
            // Get the consultation to determine ownership
            var consultation = await _consultationService.GetConsultationByIdAsync(consultationId);
            if (consultation == null)
            {
                return NotFound();
            }
            
            // Check if the user is authorized to cancel this consultation
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            
            // If teacher, check if they own the consultation
            if (User.IsInRole("Teacher") && consultation.TeacherId.ToString() != userId)
            {
                return Forbid();
            }
            
            // If student, check if they are registered for the consultation
            if (User.IsInRole("Student"))
            {
                // Check if student is registered for the consultation
                bool isRegistered = await _consultationService.IsStudentRegisteredForConsultationAsync(
                    consultationId, Guid.Parse(userId));
                
                if (!isRegistered)
                {
                    return Forbid();
                }
            }
            
            await _consultationService.CancelConsultationAsync(consultationId);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanEditConsultation")]
        public async Task<IActionResult> UpdateConsultation(Guid id, [FromBody] ConsultationDto dto)
        {
            // Get the current user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            // Only allow the owner teacher or admin to update
            var consultation = await _consultationService.GetConsultationByIdAsync(id);
            if (consultation == null)
            {
                return NotFound();
            }
            
            // Use authorization service for resource-based authorization
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, 
                consultation, 
                new WebAPI.Authorization.ResourceOwnerRequirement("Administrator"));
            
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            
            // Update logic
            await _consultationService.UpdateConsultationAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteConsultation")]
        public async Task<IActionResult> DeleteConsultation(Guid id)
        {
            // Get the current user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            // Only allow the owner teacher or admin to delete
            var consultation = await _consultationService.GetConsultationByIdAsync(id);
            if (consultation == null)
            {
                return NotFound();
            }
            
            // Use authorization service for resource-based authorization
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, 
                consultation, 
                new WebAPI.Authorization.ResourceOwnerRequirement("Administrator"));
            
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            
            // Use soft delete instead of hard delete
            await _consultationService.SoftDeleteConsultationAsync(id);
            return NoContent();
        }

        [HttpGet("including-deleted")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> GetAllConsultationsIncludingDeleted()
        {
            var consultations = await _consultationService.GetAllConsultationsIncludingDeletedAsync();
            return Ok(consultations);
        }

        [HttpGet("upcoming")]
        [AllowAnonymous] // Allow anyone to get upcoming consultations
        public async Task<IActionResult> GetUpcomingConsultations()
        {
            try
            {
                _logger.LogInformation("Request received for upcoming consultations");
                var allConsultations = await _consultationService.GetAllConsultationsAsync();
                var now = DateTime.UtcNow;
                var upcoming = allConsultations
                    .Where(c => c != null && c.ScheduledAt > now && (c.Status == null || c.Status.ToLower() != "cancelled"))
                    .OrderBy(c => c.ScheduledAt)
                    .Select(c => new {
                        id = c.Id.ToString(),
                        title = c.Title ?? string.Empty,
                        topic = c.Topic ?? string.Empty,
                        description = c.Description ?? string.Empty,
                        scheduledAt = c.ScheduledAt.ToString("o"),
                        durationMinutes = c.DurationMinutes,
                        status = c.Status ?? string.Empty,
                        location = c.Location ?? string.Empty,
                        isOnline = c.IsOnline,
                        teacherId = c.TeacherId.ToString(),
                        teacherName = c.TeacherName ?? string.Empty
                    })
                    .ToList();
                var jsonText = System.Text.Json.JsonSerializer.Serialize(upcoming);
                _logger.LogInformation($"Returning {upcoming.Count} upcoming consultations");
                return Content(jsonText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error getting upcoming consultations: {Message}", ex.Message);
                return Content("[]", "application/json");
            }
        }

        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher,Administrator,Admin")]
        public async Task<IActionResult> GetTeacherConsultations()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token for GetTeacherConsultations");
                    return Unauthorized();
                }

                var teacherId = Guid.Parse(userId);
                var allConsultations = await _consultationService.GetTeacherConsultationsAsync(teacherId);
                
                // Map to anonymous objects to ensure clean serialization
                var result = allConsultations
                    .Where(c => c != null)
                    .Select(c => new 
                    {
                        id = c.Id.ToString(),
                        title = c.Title ?? string.Empty,
                        topic = c.Topic ?? string.Empty,
                        description = c.Description ?? string.Empty,
                        scheduledAt = c.ScheduledAt.ToString("o"), // ISO 8601 format
                        durationMinutes = c.DurationMinutes,
                        status = c.Status ?? string.Empty,
                        location = c.Location ?? string.Empty,
                        isOnline = c.IsOnline,
                        teacherId = c.TeacherId.ToString(),
                        teacherName = c.TeacherName ?? string.Empty
                    })
                    .ToList();
                
                var jsonText = System.Text.Json.JsonSerializer.Serialize(result);
                return Content(jsonText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher consultations");
                return Content("[]", "application/json");
            }
        }

        [HttpGet("student")]
        [Authorize(Roles = "Student,Administrator,Admin")]
        public async Task<IActionResult> GetStudentConsultations()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token for GetStudentConsultations");
                    return Unauthorized();
                }
                
                var studentId = Guid.Parse(userId);
                var allConsultations = await _consultationService.GetStudentConsultationsAsync(studentId);
                
                // Map to anonymous objects to ensure clean serialization
                var result = allConsultations
                    .Where(c => c != null)
                    .Select(c => new 
                    {
                        id = c.Id.ToString(),
                        title = c.Title ?? string.Empty,
                        topic = c.Topic ?? string.Empty,
                        description = c.Description ?? string.Empty,
                        scheduledAt = c.ScheduledAt.ToString("o"), // ISO 8601 format
                        durationMinutes = c.DurationMinutes,
                        status = c.Status ?? string.Empty,
                        location = c.Location ?? string.Empty,
                        isOnline = c.IsOnline,
                        teacherId = c.TeacherId.ToString(),
                        teacherName = c.TeacherName ?? string.Empty
                    })
                    .ToList();
                
                var jsonText = System.Text.Json.JsonSerializer.Serialize(result);
                return Content(jsonText, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student consultations");
                return Content("[]", "application/json");
            }
        }
    }
}
