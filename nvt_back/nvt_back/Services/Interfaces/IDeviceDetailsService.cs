using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.DTOs.DeviceDetailsDTO;
using nvt_back.Model.Devices;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceDetailsService
    {
        public Task<PageResultDTO<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId, int page, int size);
    
        public Task<object> GetById(int id);
        Task<List<SharedDevicesDTO>> GetSharedDevice(int id);
        Task<List<PropertyDTO>> GetSharedPropertyDetails(int userId);
        Task<List<DeviceDetailsDTO>> GetSharedDevicesDetails(int id);
        Task<List<SharedDevicesDTO>> GetSharedDeviceRequests(int id);
    }
}
