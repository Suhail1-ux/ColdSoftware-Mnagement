using ColdStoreManagement.BLL.Models.Company;
using ColdStoreManagement.BLL.Models.Crate;
using ColdStoreManagement.DAL.Helper;
using ColdStoreManagement.DAL.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ColdStoreManagement.DAL.Services.Implementation
{
    public class CrateService(SQLHelperCore sql) : BaseService(sql), ICrateService
    {

        // ================== EXISTS ==================

        public async Task<bool> DoesCrateTypeExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM crtypes WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;

        public async Task<bool> DoesCrateFlagExistAsync(string name)
            => (await _sql.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM CrateFlags WHERE name=@Name",
                CommandType.Text,
                new SqlParameter("@Name", name))) > 0;


        // ================== CRUD ==================

        public async Task<bool> AddCrateTypeAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.crtypes (id,name,Crqty) 
                        values((select isnull(max(id+1),1) from crtypes),@Crname,@Crqty)",
                new SqlParameter("@Crname", m.Crname),
                new SqlParameter("@Crqty", m.Crqty));

            return true;
        }

        public async Task<bool> UpdateCrateTypeAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "update dbo.crtypes set name=@Crname,Crqty=@Crqty where  id=@Id",
                new SqlParameter("@id", m.Crid),
                new SqlParameter("@Crname", m.Crname),
                new SqlParameter("@Crqty", m.Crqty));
            return true;
        }

        public async Task<bool> DeleteCrateTypeAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE crtypes WHERE id=@id",
                new SqlParameter("@id", id));
            return true;
        }

        public async Task<bool> AddCrateFlagAsync(CrateFlags model)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"insert into dbo.CrateFlags (id,name,ftype,srtype)  
                    values((select isnull(max(id+1),1) from CrateFlags),@Cfname,@Cfstat,@Cflag)",

                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }
        public async Task<bool> UpdateCrateFlagAsync(CrateFlags model)
        {
            const string query = "update dbo.CrateFlags set name=@Cfname,ftype=@Cfstat,srtype=@Cflag where id=@Id";
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                query,
                new SqlParameter("@id", model.Id),
                new SqlParameter("@Cfname", model.Name),
                new SqlParameter("@Cfstat", model.Ftype),
                new SqlParameter("@Cflag", model.Srtype));

            return true;
        }
        public async Task<bool> DeleteCrateFlagAsync(int id)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                "DELETE CrateFlags WHERE id=@id",
                new SqlParameter("@id", id));

            return true;
        }
        public async Task<bool> DeleteCrateTypes(int id)
        {
            await _sql.ExecuteNonQueryAsync(
               CommandType.Text,
               "DELETE dbo.crtypes WHERE id=@id",
               new SqlParameter("@id", id));

            return true;
        }
        public async Task<bool> UpdateCratetypes(CrateType crateType)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.Text,
                @"update dbo.crtypes set name=@Crname,Crqty=@Crqty where id=@Id",
                new SqlParameter("@Id", crateType.Id),
                new SqlParameter("@Crname", crateType.Name),
                new SqlParameter("@Crqty", crateType.Crqty));

            return true;
        }


        public async Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Crate Issue' AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesOutByDateAsync(DateTime from, DateTime to)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag IN ('Empty Returned','Petty Returned')
                  AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesEmptyByDateAsync(DateTime from, DateTime to)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Empty Receive'
                  AND flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesAdjustmentAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,qty,remarks
                  FROM crateadjustment
                  WHERE flagdeleted=0
                  ORDER BY cid DESC");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();
            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueRemarks = r["remarks"]?.ToString()
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> GetDailyCratesAdjByDateAsync(DateTime from, DateTime to)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,qty,remarks
                  FROM crateadjustment
                  WHERE flagdeleted=0
                  AND CONVERT(date,Dated) BETWEEN @f AND @t
                  ORDER BY cid DESC",
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueRemarks = r["remarks"]?.ToString()
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GetCrateSummarySubGrowerAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CratesummarySub",
                new SqlParameter("@partyname", party));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT Agreement,crateissue,Cratereceive,EmptyReceive FROM CrateAnalysissummary");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();
            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("Agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("Cratereceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GenerateCrateReportAsync(CompanyModel filter)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.StoredProcedure,
                "GenerateCrateReport",
                new SqlParameter("@FromDate", filter.CrateDatefrom),
                new SqlParameter("@ToDate", filter.CrateDateto),
                new SqlParameter("@PartyId", filter.Partyid),
                new SqlParameter("@GrowerId", filter.Growerid),
                new SqlParameter("@CrateMark", filter.CrissueMark),
                new SqlParameter("@Flag", filter.CrissueFlag));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> CheckCratesAgreementOnVQtyAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesAgreementvqty",
                new SqlParameter("@partyname", party));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT SUM(AgreeQty)-SUM(receiveQty) AS Bookqty FROM CHECKCRATEQTY");
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    PrebookQty = r.Field<decimal>("Bookqty")
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> CheckCratesPartyOutAsync(string party, string flag)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartyout",
                new SqlParameter("@partyname", party),
                new SqlParameter("@trflag", flag));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT AGREEMENT,Receive,returned,Balance FROM CrateAnalysis");
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("AGREEMENT"),
                    CrateReceive = r.Field<decimal>("Receive"),
                    EmptyReceive = r.Field<decimal>("returned"),
                    CrateAvailable = r.Field<decimal>("Balance")
                })
                .ToList();
        }

        public async Task<List<CompanyModel>> CheckCratesPartyEmptyAsync(string party)
            => await CheckCratesPartyAsync(party);

        public async Task<List<CompanyModel>> CheckCratesPartySubAsync(string party, string grower)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartysub",
                new SqlParameter("@partyname", party),
                new SqlParameter("@growername", grower));

            return await CheckCratesPartyAsync(party);
        }

        public async Task<List<CompanyModel>> CheckCratesPartySubEmptyAsync(string party, string grower)
            => await CheckCratesPartySubAsync(party, grower);

        public async Task<List<CompanyModel>> CheckCratesPartySubOutAsync(string party, string grower, string flag)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesPartysubout",
                new SqlParameter("@partyname", party),
                new SqlParameter("@growername", grower),
                new SqlParameter("@trflag", flag));

            return await CheckCratesPartyAsync(party);
        }



        // ================== ISSUE ==================

        public async Task<CompanyModel?> AddCrateIssueAsync(CompanyModel m)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "addCrateIssueproc",
                new SqlParameter("@Cdate", m.CrateIssueDated),
                new SqlParameter("@Partyname", m.GrowerGroupName),
                new SqlParameter("@GrowerName", m.GrowerName),
                new SqlParameter("@challanName", m.ChallanName),
                new SqlParameter("@userid", m.UserId),
                new SqlParameter("@crateMark", m.CrissueMark),
                new SqlParameter("@Vehno", m.Vehno),
                new SqlParameter("@Remarks", m.CrissueRemarks),
                new SqlParameter("@Qty", m.CrissueQty));

            await FillValidationAsync(m);
            return m;
        }

        public async Task<int> GetMaxCrateIssueIdAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT MAX(cid) AS cid FROM crateissue");

            if (ds.Tables.Count == 0)
                return 0;

            return ds.Tables[0].Rows.Count == 0
                ? 0
                : Convert.ToInt32(ds.Tables[0].Rows[0]["cid"]);
        }
        public async Task<List<CompanyModel>> GetAllCrateMarksAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT DISTINCT CrateMark FROM Crateissue");
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueMark = r["CrateMark"]?.ToString()
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> GetDailyCratesAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Crate Issue' AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesOutAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag IN ('Empty Returned','Petty Returned')
                  AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetDailyCratesEmptyAsync()
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                  FROM crateissue
                  WHERE trflag='Empty Receive'
                  AND flagdeleted=0
                  AND CONVERT(date,Dated)=CONVERT(date,GETDATE())
                  ORDER BY cid DESC");

            return MapCrateIssue(ds);
        }

        public async Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to, string flag)
        {
            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT cid,Trno,Dated,CrateMark,qty,trflag,Remarks
                      FROM crateissue
                      WHERE trflag=@flag AND flagdeleted=0
                      AND CONVERT(date,Dated) BETWEEN @f AND @t
                      ORDER BY cid DESC",
                new SqlParameter("@flag", flag),
                new SqlParameter("@f", from),
                new SqlParameter("@t", to));

            return MapCrateIssue(ds);
        }
        public async Task<List<CompanyModel>> GetCrateSummaryMainAsync()
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CratesummaryMain");

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                @"SELECT partyid,Agreement,crateissue,Cratereceive,EmptyReceive,
                  (crateissue)-(Cratereceive+EmptyReceive) AS Balance
                  FROM CrateAnalysissummary");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    Partyid = r.Field<int>("partyid"),
                    CrateAgreement = r.Field<decimal>("Agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("Cratereceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive"),
                    CrateBalance = r.Field<decimal>("Balance")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> CheckCratesPartyAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesParty",
                new SqlParameter("@partyname", party));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT agreement,crateissue,CrateReceive,EmptyReceive,availqty FROM CrateAnalysis");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrateAgreement = r.Field<decimal>("agreement"),
                    CrateIssue = r.Field<decimal>("crateissue"),
                    CrateReceive = r.Field<decimal>("CrateReceive"),
                    EmptyReceive = r.Field<decimal>("EmptyReceive"),
                    CrateAvailable = r.Field<decimal>("availqty")
                })
                .ToList();
        }
        public async Task<List<CompanyModel>> CheckCratesAgreementAsync(string party)
        {
            await _sql.ExecuteNonQueryAsync(
                CommandType.StoredProcedure,
                "CheckCratesAgreement",
                new SqlParameter("@partyname", party));

            var ds = await _sql.ExecuteDatasetAsync(
                CommandType.Text,
                "SELECT SUM(AgreeQty)-SUM(receiveQty) AS Bookqty FROM CHECKCRATEQTY");

            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    PrebookQty = r.Field<decimal>("Bookqty")
                })
                .ToList();
        }


        private List<CompanyModel> MapCrateIssue(DataSet ds)
        {
            if (ds.Tables.Count == 0)
                return new List<CompanyModel>();

            return ds.Tables[0].AsEnumerable()
                .Select(r => new CompanyModel
                {
                    CrissueId = r.Field<int>("cid"),
                    CrateCrn = r["Trno"]?.ToString(),
                    CrateIssueDated = r.Field<DateTime>("Dated"),
                    CrissueMark = r["CrateMark"]?.ToString(),
                    CrissueQty = r.Field<decimal>("qty"),
                    CrissueFlag = r["trflag"]?.ToString(),
                    CrissueRemarks = r["Remarks"]?.ToString()
                })
                .ToList();
        }


    }
}
