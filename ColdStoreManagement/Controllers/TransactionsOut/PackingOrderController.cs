using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackingOrderController(IPackingOrderService packingOrderService) : ControllerBase
    {
        private readonly IPackingOrderService _packingOrderService = packingOrderService;

        [HttpGet("GetPackingOrderPriv")]
        public async Task<IActionResult> GetPackingOrderPriv(string Ugroup)
        {
            var result = await _packingOrderService.GetPackingOrderPriv(Ugroup);
            return Ok(result);
        }

        [HttpGet("GetAllOrderby")]
        public async Task<IActionResult> GetAllOrderby(int UnitId)
        {
            var result = await _packingOrderService.GetAllOrderby(UnitId);
            return Ok(result);
        }

        [HttpPost("generateCompletepackingorderOpen")]
        public async Task<IActionResult> generateCompletepackingorderOpen([FromBody] CompanyModel EditModel, int unit)
        {
            var result = await _packingOrderService.generateCompletepackingorderOpen(EditModel, unit);
            return Ok(result);
        }

        [HttpPost("UpdatePackingorderStatus")]
        public async Task<IActionResult> UpdatePackingorderStatus(int id, [FromBody] CompanyModel companyModel)
        {
            var result = await _packingOrderService.UpdatePackingorderStatus(id, companyModel);
            return Ok(result);
        }

        [HttpPost("DeletePackingOrder")]
        public async Task<IActionResult> DeletePackingOrder(int selectedGrowerId, [FromBody] CompanyModel companyModel)
        {
            var result = await _packingOrderService.DeletePackingOrder(selectedGrowerId, companyModel);
            return Ok(result);
        }

        [HttpPost("AssignpackingId")]
        public async Task<IActionResult> AssignpackingId(string uname, int packingorder, [FromBody] CompanyModel EditModel)
        {
            var result = await _packingOrderService.AssignpackingId(uname, packingorder, EditModel);
            return Ok(result);
        }

        [HttpGet("GetAllPackingOrders")]
        public async Task<IActionResult> GetAllPackingOrders(int UnitId)
        {
            var result = await _packingOrderService.GetAllPackingOrders(UnitId);
            return Ok(result);
        }

        [HttpGet("GetallDrafts")]
        public async Task<IActionResult> GetallDrafts(int UnitId)
        {
            var result = await _packingOrderService.GetallDrafts(UnitId);
            return Ok(result);
        }
    }
}
