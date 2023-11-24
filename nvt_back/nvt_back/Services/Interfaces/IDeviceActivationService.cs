using nvt_back.DTOs.DeviceCommunication;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceActivationService
    {
        public void ActivateDevice(int deviceId);

        public void DeactivateDevice(int deviceId);
    }
}
