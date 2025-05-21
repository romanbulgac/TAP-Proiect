using BusinessLayer.Interfaces;
using BusinessLayer.ModelDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TestsController : ControllerBase
    {
        private readonly IAsyncTestService _testService;

        public TestsController(IAsyncTestService testService)
        {
            _testService = testService;
        }

        // GET: api/Tests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestModelDto>>> GetAllTests()
        {
            var tests = await _testService.GetAllAsync();
            return Ok(tests);
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestModelDto>> GetTest(Guid id)
        {
            var test = await _testService.GetByIdAsync(id);
            
            if (test == null)
                return NotFound();
                
            return Ok(test);
        }

        // POST: api/Tests
        [HttpPost]
        public async Task<ActionResult<TestModelDto>> CreateTest(TestModelDto testDto)
        {
            try
            {
                var id = await _testService.CreateAsync(testDto);
                
                // Return the created test
                var createdTest = await _testService.GetByIdAsync(id);
                return CreatedAtAction(nameof(GetTest), new { id = createdTest.Id }, createdTest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(Guid id, TestModelDto testDto)
        {
            try
            {
                await _testService.UpdateAsync(id, testDto);
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

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(Guid id)
        {
            try
            {
                await _testService.DeleteAsync(id);
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
        
        // GET: api/Tests/including-deleted
        [HttpGet("including-deleted")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<TestModelDto>>> GetAllTestsIncludingDeleted()
        {
            var tests = await _testService.GetAllIncludingDeletedAsync();
            return Ok(tests);
        }
    }
}
