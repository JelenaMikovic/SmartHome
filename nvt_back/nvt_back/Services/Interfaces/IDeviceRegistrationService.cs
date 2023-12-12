using nvt_back.DTOs;

namespace nvt_back.Services
{
    public interface IDeviceRegistrationService
    {
        public Task Add(DeviceRegistrationDTO dto);
    }
}
