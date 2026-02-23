using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockAggregateController : ControllerBase
    {
        private readonly IStockAggregateService _service;

        public StockAggregateController(IStockAggregateService service)
        {
            _service = service;
        }

        [HttpGet("GetStockAggregate")]
        public async Task<IActionResult> GetStockAggregate()
        {
            var result = await _service.GetStockAggregateDataAsync();
            return Ok(result);
        }

        [HttpGet("GetStockChamber/{growerId}")]
        public async Task<IActionResult> GetStockChamber(int growerId)
        {
            var result = await _service.GetStockChamberDataAsync(growerId);
            return Ok(result);
        }
    }
}
