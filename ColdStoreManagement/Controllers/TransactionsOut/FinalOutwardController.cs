using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinalOutwardController : ControllerBase
    {
        private readonly IFinalOutwardService _finalOutwardService;

        public FinalOutwardController(IFinalOutwardService finalOutwardService)
        {
            _finalOutwardService = finalOutwardService;
        }

        [HttpGet("FInaloutwardPriv")]
        public async Task<IActionResult> FInaloutwardPriv(string Ugroup)
        {
            var result = await _finalOutwardService.FInaloutwardPriv(Ugroup);
            return Ok(result);
        }

        [HttpPost("AddFinaloutward")]
        public async Task<IActionResult> AddFinaloutward([FromBody] CompanyModel EditModel, int unit)
        {
            var result = await _finalOutwardService.AddFinaloutward(EditModel, unit);
            return Ok(result);
        }

        [HttpPost("UpdateFinaloutward")]
        public async Task<IActionResult> UpdateFinaloutward([FromBody] UpdateFinalOutwardRequest request)
        {
            var result = await _finalOutwardService.UpdateFinaloutward(request.CheckedPellets, request.BaseEditModel, request.Unit);
            return Ok(result);
        }
    }

    public class UpdateFinalOutwardRequest
    {
        public List<CompanyModel> CheckedPellets { get; set; }
        public CompanyModel BaseEditModel { get; set; }
        public int Unit { get; set; }
    }
}
