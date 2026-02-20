using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/service-types")]
    [Authorize]
    public class ServiceTypeController(
        IServiceTypeService serviceTypeService,
        ILogger<ServiceTypeController> logger) : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService = serviceTypeService;
        private readonly ILogger<ServiceTypeController> _logger = logger;

        /// <summary>
        /// Get all service-types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _serviceTypeService.GetAllServices();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all service types");
                throw;
            }
        }

        /// <summary>
        /// Get specific service-type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var service = await _serviceTypeService.GetServiceById(id);
                if (service == null)
                    return NotFound(new { message = "Service type not found" });

                return Ok(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service type {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// check service-types exists or not by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("exists/{name}")]
        public async Task<IActionResult> Exists(string name)
        {
            try
            {
                var exists = await _serviceTypeService.DoesServiceExistAsync(name);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking service type existence: {Name}", name);
                throw;
            }
        }

        /// <summary>
        /// Get service-types agreement details {purchase}
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpGet("agreement/{purchase}")]
        public async Task<IActionResult> GetFromAgreement(string purchase)
        {
            try
            {
                var result = await _serviceTypeService.GetServicesFromAgreement(purchase);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services from agreement {Purchase}", purchase);
                throw;
            }
        }

        /// <summary>
        /// Add new service-types
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceTypesModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exists = await _serviceTypeService.DoesServiceExistAsync(model.ServiceName);
                if (exists)
                    return Conflict(new { message = "Service type already exists" });

                await _serviceTypeService.AddService(model);
                return Ok(new { message = "Service type created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service type");
                throw;
            }
        }

        /// <summary>
        /// update existing service-types
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceTypesModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _serviceTypeService.UpdateService(id, model);
                return Ok(new { message = "Service type updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service type {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete service-types
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceTypeService.DeleteService(id);
                return Ok(new { message = "Service type deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service type {Id}", id);
                throw;
            }
        }
    }
}
