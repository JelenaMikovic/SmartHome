using nvt_back.DTOs.DeviceDetailsDTO;
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
        public Task ToggleState(int id, string status);
        Task<IEnumerable<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId, int page, int size);
        Task<IEnumerable<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId);
        public Task<int> GetDeviceCountForProperty(int propertyId);
        Task<object> GetDetailsById(int id);
        public Task ToggleRegime(int deviceId, string value);
        public Task SaveChanges(Device device);

        public Task ToggleCommand(int deviceId, string type, string value);
        public Task UpdateGateAllowedPlates(int deviceId, string value, bool isAdd);
        Task<List<int>> GetPropertyIdsWithHomeBatteries();
        Task<List<HomeBattery>> GetAllBatteriesForPropertyId(int propertyId);
        Task<List<Device>> GetConsumingPowerDevicesForProperty(int propertyId);
        Task AddSchedule(AirConditionerSchedule schedule);
        public Task RemoveSchedule(int scheduleId);
        Task<List<AirConditionerSchedule>> GetDeviceSchedules(int deviceId);
        Task AddSharedDevice(SharedDevices devices);
        Task UpdateSharedDevices(int id, SharedStatus status);
        Task<List<SharedDevices>> GetSharedDevces(int userId);
        Task<List<SharedDevices>> GetSharedDevicesOwner(int id);
        Task<DeviceDetailsDTO> GetSharedDeviceDetails(int id);
    }
}
