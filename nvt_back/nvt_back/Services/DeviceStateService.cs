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
                case "open":
                    return await handleGateOpen(dto, userId);
                case "private":
                    return await handleGatePrivate(dto, userId);
                case "addplate":
                    await handleAddGateAllowedPlate(dto, userId, true);
                    return true;
                case "removeplate":
                    await handleAddGateAllowedPlate(dto, userId, false);
                    return true;
            }

            return false;
        }

        private async Task handleAddGateAllowedPlate(CommandDTO dto, int userId, bool isAdd)
        {
            VehicleGate gate = await validateGateCommand(dto);

            if (isAdd)
            {
                await _mqttClientService.PublishCommandUpdate(dto.DeviceId, dto.Action, dto.Value, userId);
            } else
            {
                await _mqttClientService.PublishCommandUpdate(dto.DeviceId, dto.Action, dto.Value, userId);
            }
        }

        private async Task<VehicleGate> validateGateCommand(CommandDTO dto)
        {
            var device = await _deviceRepository.GetById(dto.DeviceId);

            if (device.DeviceType != DeviceType.VEHICLE_GATE)
                throw new Exception("The device doesn't have this command option.");


            VehicleGate gate = (VehicleGate)device;
            Console.WriteLine(gate.IsOpened.ToString().ToLower() + " " + dto.Value.ToLower());

            return gate;
        }

        private async Task<bool> handleGatePrivate(CommandDTO dto, int userId)
        {
            VehicleGate gate = await validateGateCommand(dto);
            
            if (gate.IsPrivateModeOn.ToString().ToLower() != dto.Value.ToLower())
            {
                await _mqttClientService.PublishCommandUpdate(dto.DeviceId, dto.Action, dto.Value, userId);
                return true;
            }

            return false;
        }

        private async Task<bool> handleGateOpen(CommandDTO dto, int userId)
        {
            VehicleGate gate = await validateGateCommand(dto);

            if (gate.IsOpened.ToString().ToLower() != dto.Value.ToLower())
            {
                await _mqttClientService.PublishCommandUpdate(dto.DeviceId, dto.Action, dto.Value, userId);
                return true;
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
            else if (device.DeviceType == DeviceType.AC)
            {
                AirConditioner ac = (AirConditioner)device;
                if (ac.IsOn == isTurnedOn)
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

        public async Task<bool> ChangeMode(CommandDTO dto, int id)
        {
            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), dto.DeviceType, true);
            if (type == DeviceType.AC)
            {
                return await ChangeAcMode(dto, id);
            } else
            {
                throw new Exception("The device type doesnt support the given regime");
            }
        }

        private async Task<bool> ChangeAcMode(CommandDTO dto, int userId)
        {
            var ac = (AirConditioner)(await _deviceRepository.GetById(dto.DeviceId));

            if (ac == null)
                throw new Exception("The ac with given id doesn't exist.");

            AirConditionerMode mode;
            try
            {
                mode = (AirConditionerMode)Enum.Parse(typeof(AirConditionerMode), dto.Value, true);
            }
            catch (ArgumentException)
            {
                throw new Exception("The ac doesnt support the given regime: " + dto.Value);
            }

            if (!ac.SupportedModes.Contains(mode))
            {
                throw new Exception("The air conditioner doesn't support the given mode: " + dto.Value);
            }

            if (mode != ac.CurrentMode)
            {
                await _mqttClientService.PublishModeUpdate(dto.DeviceId, mode.ToString(), userId);
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeTemperature(CommandDTO dto, int id)
        {
            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), dto.DeviceType, true);
            if (type == DeviceType.AC)
            {
                return await ChangeAcTemperature(dto, id);
            }
            else
            {
                throw new Exception("The device type doesnt support the given control");
            }
        }

        private async Task<bool> ChangeAcTemperature(CommandDTO dto, int userId)
        {
            var ac = (AirConditioner)(await _deviceRepository.GetById(dto.DeviceId));

            if (ac == null)
                throw new Exception("The ac with given id doesn't exist.");

            double temperature;
            try
            {
                temperature = double.Parse(dto.Value);
            }
            catch (ArgumentException)
            {
                throw new Exception("The ac doesnt support the given value: " + dto.Value);
            }

            if (ac.MinTemperature > temperature || ac.MaxTemperature < temperature )
            {
                throw new Exception("The air conditioner doesn't support the given value: " + dto.Value);
            }

            if (temperature != ac.CurrentTemperature)
            {
                await _mqttClientService.PublishTemperatureUpdate(dto.DeviceId, temperature.ToString(), userId);
                return true;
            }

            return false;
        }

    }
}
