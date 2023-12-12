using nvt_back.Model.Devices;

namespace nvt_back.Repositories.Interfaces
{
    public interface IDeviceRegistrationRepository
    {
        public Task Add(Device device);
    }
}
