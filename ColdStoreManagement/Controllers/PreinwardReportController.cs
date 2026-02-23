using ColdStoreManagement.BLL.Models.Reports;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreinwardReportController : ControllerBase
    {
        private readonly IPreinwardReportService _service;

        public PreinwardReportController(IPreinwardReportService service)
        {
            _service = service;
        }

        [HttpPost("GetReportData")]
        public async Task<IActionResult> GetReportData([FromBody] ReportRequestModel request)
        {
            var result = await _service.GetReportDataAsync(request);
            return Ok(result);
        }
    }
}
