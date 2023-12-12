using nvt_back.Model.Devices;
using nvt_back.Mqtt;

namespace nvt_back.Repositories.Interfaces
{
    public interface IDeviceRepository
    {
        public Task<Device> GetById(int deviceId);
        public Task UpdateOnlineStatus(int deviceId, bool activate);
        public Task<List<Device>> GetOnlineDevices();
        public Task<List<Device>> GetAll();
        public Task UpdateLatestHeartbeat(Heartbeat heartbeat, DateTime time);
    }
}
