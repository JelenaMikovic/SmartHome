using Newtonsoft.Json;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using System.Reactive.Joins;

namespace nvt_back.Services
{

    public class DeviceOnlineStatusService : IDeviceOnlineStatusService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeviceOnlineStatusService(IDeviceRepository deviceRepository, IServiceScopeFactory scopedFacotry)
        {
            _deviceRepository = deviceRepository;
            _scopeFactory = scopedFacotry;
        }

        public async Task<List<Device>> GetOnlineDevices()
        {
            return await _deviceRepository.GetOnlineDevices();
        }

        public async Task UpdateOnlineStatus(int deviceId, bool isOnline)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //battery.CurrentCharge = data.Value;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.UpdateOnlineStatus(deviceId, isOnline);
            }
        }

        public async Task<bool> HasDeviceOnlineStatusChanged(int id, bool newStatus)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //battery.CurrentCharge = data.Value;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                Device device = await repository.GetById(id);
                return device.IsOnline != newStatus;
            }
            //Device device = await _deviceRepository.GetById(id);
            //return device.IsOnline != newStatus;
        }

        public async Task UpdateLatestHeartbeat(Heartbeat heartbeat, DateTime time)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //battery.CurrentCharge = data.Value;
                var serviceProvider = scope.ServiceProvider;
                var repository = serviceProvider.GetRequiredService<IDeviceRepository>();
                await repository.UpdateLatestHeartbeat(heartbeat, time);
            }
        }
    }
}
