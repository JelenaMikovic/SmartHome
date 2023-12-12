using nvt_back.Model.Devices;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceService 
    {
        public Task<List<Device>> GetAll();
    }
}
