using nvt_back.Model.Devices;

namespace nvt_back.Repositories.Interfaces
{
    public interface IDeviceRepository
    {
        public Device GetById(int deviceId);
        public Device ChangeOnlineStatus(int deviceId, bool activate);
        public List<Device> GetOnlineDevices();
    }
}
