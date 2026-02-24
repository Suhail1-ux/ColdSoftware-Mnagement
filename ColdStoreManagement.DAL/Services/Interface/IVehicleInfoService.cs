using ColdStoreManagement.BLL.Models;
using ColdStoreManagement.BLL.Models.DTOs;

namespace ColdStoreManagement.DAL.Services.Interface
{
    public interface IVehicleInfoService
    {
        Task<List<VehInfoModel>> GetAllVehGroup();
        Task<VehInfoModel?> Getvehid(int vehid);
        Task<VehicleDto?> Addveh(VehInfoModel model);
        Task<VehicleDto?> UpdateVeh(VehInfoModel model);
        Task<bool> UpdatevehStatus(int id);
        Task<VehicleDto?> DeleteVeh(int id, VehicleDto model);
        Task<List<ItemDto>> GetallItemGroup();
    }
}
