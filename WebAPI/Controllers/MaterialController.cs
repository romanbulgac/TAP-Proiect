using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        // Dependency injection for material service
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public IActionResult GetAllMaterials()
        {
            var materials = _materialService.GetAll();
            return Ok(materials);
        }

        [HttpGet("{id}")]
        public IActionResult GetMaterialById(Guid id)
        {
            var material = _materialService.GetById(id);
            if (material == null)
                return NotFound();
            return Ok(material);
        }

        [HttpPost]
        public IActionResult CreateMaterial(MaterialDto model)
        {
            _materialService.Create(model);
            return CreatedAtAction(nameof(GetMaterialById), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMaterial(Guid id, MaterialDto model)
        {
            if (id != model.Id)
                return BadRequest();

            _materialService.Update(id, model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMaterial(Guid id)
        {
            _materialService.Delete(id);
            return NoContent();
        }
    }
}