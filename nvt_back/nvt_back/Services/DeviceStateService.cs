using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{
    public class DeviceStateService : IDeviceStateService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMqttClientService _mqttClientService;

        public DeviceStateService(IDeviceRepository deviceRepository, IMqttClientService mqttClientService)
        {
            _deviceRepository = deviceRepository;
            _mqttClientService = mqttClientService;
        }
        public async Task<bool> Toggle(int id, string status)
        {
            bool hasStatusChanged = await _deviceRepository.ToggleState(id, status);
            if (hasStatusChanged)
            {
                await _mqttClientService.PublishStatusUpdate(id, status);
                //TODO: dodati u influx upis promene statusa
            }
            return hasStatusChanged;
        }
    }
}
