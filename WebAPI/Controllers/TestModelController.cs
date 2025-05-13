using BusinessLayer.Interfaces;
using BusinessLayer.ModelDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestModelController : ControllerBase
    {
        private readonly ITestService _testService;
        public TestModelController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var models = _testService.GetAll();
            return Ok(models);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var model = _testService.GetById(id);
            if (model == null)
                return NotFound();
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TestModelDto model)
        {
            if (model == null) return BadRequest("Model cannot be null");
            if (string.IsNullOrEmpty(model.Name)) return BadRequest("Name is required");
            _testService.Create(model);
            return Ok("Successfully created");
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TestModelDto model)
        {
            if (model == null) return BadRequest("Model cannot be null");
            if (id != model.Id) return BadRequest("Mismatched ID");
            if (string.IsNullOrEmpty(model.Name)) return BadRequest("Name is required");
            _testService.Update(id, model);
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _testService.Delete(id);
            return Ok("Deleted succesfully");
        }
    }
}
