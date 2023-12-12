using nvt_back.DTOs.DeviceCommunication;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceOnlineStatusService
    {
        public Task UpdateOnlineStatus(int deviceId, bool isActive);
        public Task<List<Device>> GetOnlineDevices();
        public Task<bool> HasDeviceOnlineStatusChanged(int id, bool newStatus);
        public Task UpdateLatestHeartbeat(Heartbeat heartbeat, DateTime time);
    }
}
