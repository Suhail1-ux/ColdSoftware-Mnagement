using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class ChamberService(SQLHelperCore sql) : BaseService(sql), IChamberService
    {
        #region ---------- Chamber / Slot ----------

        public async Task<CompanyModel?> AddNewChamber(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "AddNewChamber",
                new SqlParameter("@ctype", model.ChamberType),
                new SqlParameter("@unit", model.Unitname),
                new SqlParameter("@Capacity", model.Capacity),
                new SqlParameter("@User", model.GlobalUserName)
            );

            await FillValidationAsync(model);
            return model;
        }

        public async Task<CompanyModel?> AddSlot(CompanyModel model)
        {
            if (model == null) return null;

            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "addslot",
                new SqlParameter("@Sdate", model.calendardate),
                new SqlParameter("@partyid", model.GrowerGroupName),
                new SqlParameter("@growerid", model.GrowerName),
                new SqlParameter("@Contact", model.GrowerContact),
                new SqlParameter("@Qty", model.SlotQty),
                new SqlParameter("@ttime", model.calendartime)
            );

            await FillValidationAsync(model);
            return model;
        }

        #endregion


    }
}
