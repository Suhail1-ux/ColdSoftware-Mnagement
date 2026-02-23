using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackingDraftController(IPackingDraftService packingDraftService) : ControllerBase
    {
        private readonly IPackingDraftService _packingDraftService = packingDraftService;

        [HttpPost("ValidateDraftQty")]
        public async Task<IActionResult> ValidateDraftQty([FromBody] CompanyModel companyModel)
        {
            var result = await _packingDraftService.ValidateDraftQty(companyModel);
            return Ok(result);
        }

        [HttpGet("GetallDraftno")]
        public async Task<IActionResult> GetallDraftno(int Unit)
        {
            var result = await _packingDraftService.GetallDraftno(Unit);
            return Ok(result);
        }

        [HttpGet("GetallDraftnobill")]
        public async Task<IActionResult> GetallDraftnobill(int Unit)
        {
            var result = await _packingDraftService.GetallDraftnobill(Unit);
            return Ok(result);
        }

        [HttpPost("AddFinalDraft")]
        public async Task<IActionResult> AddFinalDraft([FromBody] CompanyModel EditModel)
        {
            var result = await _packingDraftService.AddFinalDraft(EditModel);
            return Ok(result);
        }

        [HttpPost("UpdateDraftQuantity")]
        public async Task<IActionResult> UpdateDraftQuantity([FromBody] CompanyModel EditModel)
        {
            var result = await _packingDraftService.UpdateDraftQuantity(EditModel);
            return Ok(result);
        }
    }
}
