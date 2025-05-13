using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationService _consultationService;
        public ConsultationsController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var consultations = _consultationService.GetAllConsultations();
            return Ok(consultations);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var consultation = _consultationService.GetConsultationById(id);
            if (consultation == null) return NotFound();
            return Ok(consultation);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ConsultationDto dto)
        {
            var newId = _consultationService.CreateConsultation(dto);
            return CreatedAtAction(nameof(Get), new { id = newId }, dto);
        }

        [HttpPost("{id}/book")]
        public IActionResult Book(Guid id, [FromQuery] Guid studentId)
        {
            _consultationService.BookConsultation(id, studentId);
            return Ok();
        }

        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(Guid id)
        {
            _consultationService.CancelConsultation(id);
            return Ok();
        }
    }
}
