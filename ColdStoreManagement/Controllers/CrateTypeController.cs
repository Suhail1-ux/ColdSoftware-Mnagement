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
    public class CrateTypeController : BaseController
    {
        private readonly ICrateTypeService _service;

        public CrateTypeController(ICrateTypeService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllCrateTypesAsync();
            return Ok(result);
        }

        [HttpGet("Exists/{name}")]
        public async Task<IActionResult> Exists(string name)
        {
            var result = await _service.DoesCrateTypeExistAsync(name);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CrateType model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.AddCrateTypeAsync(model);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] CrateType model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateCrateTypeAsync(model);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteCrateTypeAsync(id);
            return Ok(result);
        }
    }
}
