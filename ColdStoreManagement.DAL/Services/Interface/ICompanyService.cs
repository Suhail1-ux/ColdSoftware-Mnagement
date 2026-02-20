using ColdStoreManagement.BLL.Models.Company;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface ICompanyService
    {
        // Company
        Task<CompanyModel?> GetCompanyByIdAsync(int companyId = 1);
        Task<bool> EditCompany(int id, CompanyModel companyModel);

        // Building
        Task<List<BuildingModel>> GetAllBuildingsAsync();
        Task<BuildingModel?> GetBuildingById(int id);
        Task<BuildingModel?> GetBuildingByName(string buildingName);
        Task<bool> AddBuildingAsync(BuildingModel model);
        Task<bool> UpdateBuildingAsync(int id, CompanyModel model);
        Task<bool> DeleteBuildingAsync(int id);

        

    }
}
