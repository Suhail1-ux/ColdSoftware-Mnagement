using ColdStoreManagement.BLL.Models.Product;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ProductQualityController(
        IProductQualityService qualityService,
        ILogger<ProductQualityController> logger) : BaseController
    {
        private readonly IProductQualityService _qualityService = qualityService;
        private readonly ILogger<ProductQualityController> _logger = logger;

        /// <summary>
        /// Get all product qualities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _qualityService.GetAllQualitiesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all product qualities");
                throw;
            }
        }

        /// <summary>
        /// Get product quality by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _qualityService.GetByIdAsync(id);
                if (result == null)
                    return NotFound($"Product quality with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching product quality with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Add new product quality
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductQualityModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if quality name already exists
                if (await _qualityService.DoesQualityExistAsync(model.Name))
                    return BadRequest(new { message = "Product quality name already exists" });

                var result = await _qualityService.AddQualityAsync(model);
                if (!result)
                    return BadRequest("Unable to create product quality");

                return Ok(new { message = "Product quality created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating product quality");
                throw;
            }
        }

        /// <summary>
        /// Update existing product quality
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductQualityModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _qualityService.UpdateQualityAsync(id, model);
                if (!result)
                    return NotFound($"Product quality with ID {id} not found");

                return Ok(new { message = "Product quality updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating product quality with ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete existing product quality
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _qualityService.DeleteQualityAsync(id);
                if (!result)
                    return NotFound($"Product quality with ID {id} not found");

                return Ok(new { message = "Product quality deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting product quality with ID {Id}", id);
                throw;
            }
        }
    }
}
