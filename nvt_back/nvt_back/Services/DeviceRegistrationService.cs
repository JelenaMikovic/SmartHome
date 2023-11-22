using nvt_back.DTOs;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Services
{
    public class DeviceRegistrationService : IDeviceRegistrationService
    {
        private readonly IDeviceRegistrationRepository _deviceRegistrationRepository;

        public DeviceRegistrationService(IDeviceRegistrationRepository deviceRegistrationRepository)
        {
            _deviceRegistrationRepository = deviceRegistrationRepository;
        }
        public void AddLamp(DeviceDTO dto)
        {
            if (dto is LampDTO lampDto)
            {
                Lamp lamp = new Lamp
                {
                    Name = lampDto.Name,
                    IsOnline = false,
                    PowerConsumption = lampDto.PowerConsumption,
                    PowerSource = lampDto.PowerSource,
                    IsOn = lampDto.IsOn,
                    BrightnessLevel = lampDto.BrightnessLevel,
                    Color = lampDto.Color,
                };

                this._deviceRegistrationRepository.AddLamp(lamp);
            }
        }
    }
}
