using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class CrateFlagController : BaseController
    {
        private readonly ICrateFlagService _service;

        public CrateFlagController(ICrateFlagService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllCrateFlagsAsync();
            return Ok(result);
        }

        [HttpGet("Exists/{name}")]
        public async Task<IActionResult> Exists(string name)
        {
            var result = await _service.DoesCrateFlagExistAsync(name);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CrateFlags model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.AddCrateFlagAsync(model);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] CrateFlags model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateCrateFlagAsync(model);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteCrateFlagAsync(id);
            return Ok(result);
        }
    }
}
