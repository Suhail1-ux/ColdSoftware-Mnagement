using ColdStoreManagement.BLL.Models.Bank;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class BankingController(IBankingService bankingService,
        ILogger<BankingController> logger) : BaseController
    {
        private readonly IBankingService _bankingService = bankingService;
        private readonly ILogger<BankingController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                var banks = await _bankingService.GetBanks();
                return Ok(banks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBank([FromBody] BankModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _bankingService.AddBank(model);

                if (!success)
                    return BadRequest("Failed to add bank");

                return Ok("Bank added successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _bankingService.UpdateBank(id, model);

                if (!success)
                    return BadRequest("Failed to update bank");

                return Ok("Bank updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            try
            {
                var success = await _bankingService.DeleteBank(id);

                if (!success)
                    return BadRequest("Failed to delete bank");

                return Ok("Bank deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> AddTransaction(AddTransactionRequest request)
        {
            try
            {if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _bankingService.AddTransaction(request, CurrentUserId);

                if (result?.RetFlag == "FALSE")
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("transaction")]
        public async Task<IActionResult> UpdateTransaction(UpdateTransactionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _bankingService.UpdateTransaction(request, CurrentUserId);

                if (result?.RetFlag == "FALSE")
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
