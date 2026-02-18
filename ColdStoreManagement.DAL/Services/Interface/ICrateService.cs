using ColdStoreManagement.BLL.Models;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICrateService
    {
        // ========= Crate Type =========
        Task<bool> DoesCrateTypeExistAsync(string name);
        Task<bool> AddCrateTypeAsync(CompanyModel model);
        Task<bool> UpdateCrateTypeAsync(CompanyModel model);
        Task<bool> DeleteCrateTypeAsync(int id);

        // ========= Crate Flag =========
        Task<bool> DoesCrateFlagExistAsync(string name);
        Task<bool> AddCrateFlagAsync(CompanyModel model);
        Task<bool> UpdateCrateFlagAsync(CompanyModel model);
        Task<bool> DeleteCrateFlagAsync(int id);

        // ========= Issue / Transaction =========
        Task<CompanyModel?> AddCrateIssueAsync(CompanyModel model);

        // ========= Lookups =========
        Task<int> GetMaxCrateIssueIdAsync();
        Task<List<CompanyModel>> GetAllCrateMarksAsync();

        // ========= Daily =========
        Task<List<CompanyModel>> GetDailyCratesAsync();
        Task<List<CompanyModel>> GetDailyCratesAdjustmentAsync();
        Task<List<CompanyModel>> GetDailyCratesOutAsync();
        Task<List<CompanyModel>> GetDailyCratesEmptyAsync();

        Task<List<CompanyModel>> GetDailyCratesByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesAdjByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesOutByDateAsync(DateTime from, DateTime to);
        Task<List<CompanyModel>> GetDailyCratesEmptyByDateAsync(DateTime from, DateTime to);

        // ========= Summary =========
        Task<List<CompanyModel>> GetCrateSummaryMainAsync();
        Task<List<CompanyModel>> GetCrateSummarySubGrowerAsync(string partyName);

        // ========= Reports =========
        Task<List<CompanyModel>> GenerateCrateReportAsync(CompanyModel filter);

        // ========= Checks =========
        Task<List<CompanyModel>> CheckCratesPartyAsync(string party);
        Task<List<CompanyModel>> CheckCratesAgreementAsync(string party);
        Task<List<CompanyModel>> CheckCratesAgreementOnVQtyAsync(string party);

        Task<List<CompanyModel>> CheckCratesPartyOutAsync(string party, string flag);
        Task<List<CompanyModel>> CheckCratesPartyEmptyAsync(string party);

        Task<List<CompanyModel>> CheckCratesPartySubAsync(string party, string grower);
        Task<List<CompanyModel>> CheckCratesPartySubEmptyAsync(string party, string grower);
        Task<List<CompanyModel>> CheckCratesPartySubOutAsync(string party, string grower, string flag);
    }

}
