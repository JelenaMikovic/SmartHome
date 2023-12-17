using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceDetailsService
    {
        public Task<PageResultDTO<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId, int page, int size);
    }
}
