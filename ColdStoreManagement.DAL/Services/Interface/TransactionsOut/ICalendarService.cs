using System.Collections.Generic;
using System.Threading.Tasks;
using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface.TransactionsOut
{
    public interface ICalendarService
    {
        Task<List<CompanyModel>> GetallSlots();
        Task<List<CompanyModel>> GetSlotbydate(DateTime Slotdate);
        Task<CompanyModel?> GetSlotdet(int selectedGrowerId);
        Task<bool> DeleteSlot(int STrid);
    }
}
