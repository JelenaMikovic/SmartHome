using nvt_back.DTOs;
using nvt_back.Model.Devices;
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


        public async Task<bool> ChangeRegime(CommandDTO dto, int userId)
        {
            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), dto.DeviceType, true);

            switch (dto.Action)
            {
                case "regime":
                    if (type == DeviceType.LAMP)
                        return await changeLampRegime(dto, userId);
                    break;
            }

            return false;
        }

        private async Task<bool> changeLampRegime(CommandDTO dto, int userId)
        {
            var lamp = (Lamp)(await _deviceRepository.GetById(dto.DeviceId));

            if (lamp == null)
                throw new Exception("The lamp with given id doesn't exist.");

            LampRegime regime;
            try
            {
                regime = (LampRegime)Enum.Parse(typeof(LampRegime), dto.Value, true);
            }
            catch (ArgumentException)
            {
                throw new Exception("The lamp doesnt support the given regime: " + dto.Value);
            }

            Console.WriteLine(regime + " " + lamp.Regime);
            if (regime != lamp.Regime)
            {
                await _mqttClientService.PublishRegimeUpdate(dto.DeviceId, regime.ToString(), userId);
                return true;
            }

            return false;
        }

        private async Task<bool> hasStateChanged(int id, string status)
        {
            var device = await _deviceRepository.GetById(id);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + id.ToString() + " doesn't exist!");
            bool isTurnedOn = (status == "ON");


            if (device.DeviceType == DeviceType.SOLAR_PANEL)
            {
                SolarPanel solarPanel = (SolarPanel)device;
                if (solarPanel.IsOn == isTurnedOn)
                    return false;
                else
                {
                    return true;
                }
            }
            else
            {
                Lamp lamp = (Lamp)device;
                if (lamp.IsOn == isTurnedOn)
                    return false;
                else
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Toggle(int id, string status, int userId)
        {
            bool hasStatusChanged = await hasStateChanged(id, status);
            if (hasStatusChanged)
            {
                await _mqttClientService.PublishStatusUpdate(id, status, userId);
            }
            return hasStatusChanged;
        }

        public async Task UpdateOnOff(int id, string status)
        {
            await this._deviceRepository.ToggleState(id, status);
        }
    }
}
