using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.DAL.Services.Interface.TransactionsOut;
using Microsoft.AspNetCore.Mvc;

namespace ColdStoreManagement.Controllers.TransactionsOut
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("GetallSlots")]
        public async Task<IActionResult> GetallSlots()
        {
            var result = await _calendarService.GetallSlots();
            return Ok(result);
        }

        [HttpGet("GetSlotbydate")]
        public async Task<IActionResult> GetSlotbydate(DateTime Slotdate)
        {
            var result = await _calendarService.GetSlotbydate(Slotdate);
            return Ok(result);
        }

        [HttpGet("GetSlotdet")]
        public async Task<IActionResult> GetSlotdet(int selectedGrowerId)
        {
            var result = await _calendarService.GetSlotdet(selectedGrowerId);
            return Ok(result);
        }

        [HttpDelete("DeleteSlot/{STrid}")]
        public async Task<IActionResult> DeleteSlot(int STrid)
        {
            var result = await _calendarService.DeleteSlot(STrid);
            return Ok(result);
        }
    }
}
