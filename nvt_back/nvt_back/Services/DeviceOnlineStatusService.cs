using Newtonsoft.Json;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{

    public class DeviceOnlineStatusService : IDeviceOnlineStatusService
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceOnlineStatusService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public async Task<List<Device>> GetOnlineDevices()
        {
            return await _deviceRepository.GetOnlineDevices();
        }

        public async Task UpdateOnlineStatus(int deviceId, bool isOnline)
        {
            await _deviceRepository.UpdateOnlineStatus(deviceId, isOnline);
        }

        public async Task<bool> HasDeviceOnlineStatusChanged(int id, bool newStatus)
        {
            Device device = await _deviceRepository.GetById(id);
            return device.IsOnline != newStatus;
        }

        public async Task UpdateLatestHeartbeat(Heartbeat heartbeat, DateTime time)
        {
            await _deviceRepository.UpdateLatestHeartbeat(heartbeat, time);
        }
    }
}
