using Newtonsoft.Json;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{

    public class DeviceActivationService : IDeviceActivationService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly MqttClientService _mqttClientService;

        public DeviceActivationService(IDeviceRepository deviceRepository, MqttClientService mqttClientService)
        {
            _deviceRepository = deviceRepository;
            _mqttClientService = mqttClientService;
        }
        public void ActivateDevice(int deviceId)
        {
            Device device = _deviceRepository.ChangeOnlineStatus(deviceId, true);
            if (device != null)
            {
                _mqttClientService.Subscribe(_mqttClientService.GetStatusTopicForDevice(deviceId));
                _mqttClientService.PublishActivatedStatus(deviceId);
            }
        }

        public void DeactivateDevice(int deviceId)
        {
            Device device = _deviceRepository.ChangeOnlineStatus(deviceId, false);
            if (device != null)
            {
                _mqttClientService.Unsubscribe(_mqttClientService.GetStatusTopicForDevice(deviceId));
                _mqttClientService.PublishDeactivatedStatus(deviceId);
            }
        }
    }
}
