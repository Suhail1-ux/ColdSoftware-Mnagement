using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispatchController(IDispatchService dispatchService) : ControllerBase
    {
        private readonly IDispatchService _dispatchService = dispatchService;

        [HttpGet("GetDispatchPriv")]
        public async Task<IActionResult> GetDispatchPriv(string Ugroup)
        {
            var result = await _dispatchService.GetDispatchPriv(Ugroup);
            return Ok(result);
        }

        [HttpPost("GenerateDispatchReport")]
        public async Task<IActionResult> GenerateDispatchReport(string packgrower, string Tlocation, int unit, [FromBody] CompanyModel EditModel)
        {
            var result = await _dispatchService.GenerateDispatchReport(packgrower, Tlocation, unit, EditModel);
            return Ok(result);
        }

        [HttpPost("GenerateDispatchReportComplete")]
        public async Task<IActionResult> GenerateDispatchReportComplete(string packgrower, string Tlocation, int unit, [FromBody] CompanyModel EditModel)
        {
            var result = await _dispatchService.GenerateDispatchReportComplete(packgrower, Tlocation, unit, EditModel);
            return Ok(result);
        }

        [HttpPost("ValidatePelletDispatch")]
        public async Task<IActionResult> ValidatePelletDispatch([FromBody] CompanyModel companyModel, int unit)
        {
            var result = await _dispatchService.ValidatePelletDispatch(companyModel, unit);
            return Ok(result);
        }

        [HttpPost("AddFinalDispatch")]
        public async Task<IActionResult> AddFinalDispatch([FromBody] CompanyModel EditModel, int unit)
        {
            var result = await _dispatchService.AddFinalDispatch(EditModel, unit);
            return Ok(result);
        }

        [HttpPost("Deletedispatch")]
        public async Task<IActionResult> Deletedispatch(int id, [FromBody] CompanyModel companyModel, int unit)
        {
            var result = await _dispatchService.Deletedispatch(id, companyModel, unit);
            return Ok(result);
        }
        [HttpPost("GenerateDispatchReportpend")]
        public async Task<IActionResult> GenerateDispatchReportpend(int unit, [FromBody] CompanyModel EditModel)
        {
            var result = await _dispatchService.GenerateDispatchReportpend(unit, EditModel);
            return Ok(result);
        }
    }
}
