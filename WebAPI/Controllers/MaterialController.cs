using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new InvalidOperationException("User ID not found in token."));


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterial(Guid id)
        {
            var material = await _materialService.GetMaterialByIdAsync(id);
            if (material == null) return NotFound();
            return Ok(material);
        }

        [HttpGet("consultation/{consultationId}")]
        public async Task<IActionResult> GetMaterialsForConsultation(Guid consultationId)
        {
            var materials = await _materialService.GetMaterialsForConsultationAsync(consultationId);
            return Ok(materials);
        }
        
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetMaterialsByTeacher(Guid teacherId)
        {
            var materials = await _materialService.GetMaterialsByTeacherAsync(teacherId);
            return Ok(materials);
        }


        [HttpPost]
        [Authorize(Roles = "Teacher,Administrator")]
        public async Task<IActionResult> CreateMaterial([FromBody] MaterialDto materialDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var userId = GetUserId();
            // Ensure TeacherId in DTO is set, possibly to userId if the creator is a teacher
            if (User.IsInRole("Teacher") && materialDto.TeacherId == Guid.Empty)
            {
                materialDto.TeacherId = userId;
            }
            else if (materialDto.TeacherId == Guid.Empty) // Admin creating for someone else must specify TeacherId
            {
                 return BadRequest("TeacherId must be specified when Admin creates material.");
            }


            var materialId = await _materialService.CreateMaterialAsync(materialDto, userId);
            materialDto.Id = materialId; // Set the ID in the DTO for the response
            return CreatedAtAction(nameof(GetMaterial), new { id = materialId }, materialDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher,Administrator")]
        public async Task<IActionResult> UpdateMaterial(Guid id, [FromBody] MaterialDto materialDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId();
            var success = await _materialService.UpdateMaterialAsync(id, materialDto, userId);
            if (!success) return NotFound(); // Or Forbid() if auth failed within service
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Administrator")]
        public async Task<IActionResult> DeleteMaterial(Guid id)
        {
            var userId = GetUserId();
            var success = await _materialService.DeleteMaterialAsync(id, userId);
            if (!success) return NotFound(); // Or Forbid()
            return NoContent();
        }
    }
}