using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandOrderController(IDemandOrderService demandOrderService) : ControllerBase
    {
        private readonly IDemandOrderService _demandOrderService = demandOrderService;

        [HttpPost("UpdateLotOrderQuantity")]
        public async Task<IActionResult> UpdateLotOrderQuantity([FromBody] CompanyModel EditModel)
        {
            var result = await _demandOrderService.UpdateLotOrderQuantity(EditModel);
            return Ok(result);
        }

        [HttpPost("GenerateTempLotReport")]
        public async Task<IActionResult> GenerateTempLotReport([FromBody] CompanyModel EditModel, int unit, int Tempid)
        {
            var result = await _demandOrderService.GenerateTempLotReport(EditModel, unit, Tempid);
            return Ok(result);
        }

        [HttpPost("GenerateTempLotRawReportEdit")]
        public async Task<IActionResult> GenerateTempLotRawReportEdit([FromBody] CompanyModel EditModel, int unit, int Tempid)
        {
            var result = await _demandOrderService.GenerateTempLotRawReportEdit(EditModel, unit, Tempid);
            return Ok(result);
        }
    }
}
