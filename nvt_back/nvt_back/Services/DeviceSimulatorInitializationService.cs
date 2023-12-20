using nvt_back.DTOs.Mqtt;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using System.Drawing;

namespace nvt_back.Services
{
    public class DeviceSimulatorInitializationService : IDeviceSimulatorInitializationService
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceSimulatorInitializationService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public async Task<object> Initialize(int deviceId)
        {
            Device device = await _deviceRepository.GetById(deviceId);
            Console.WriteLine(device);
            if (device == null)
            {
                throw new ArgumentException("err");
            }

            if (device.DeviceType == DeviceType.SOLAR_PANEL)
                return initializeSolarPanel(device);
            if (device.DeviceType == DeviceType.LAMP)
                return initializeLamp(device);
            if (device.DeviceType == DeviceType.AMBIENT_SENSOR)
                return initializeAmbientalSensor(device);

            return null;
        }

        private LampInitializationDTO initializeLamp(Device device)
        {
            Lamp lamp = (Lamp)device;

            Console.WriteLine("***************************" + lamp.Regime.ToString());

            return new LampInitializationDTO
            {
                Type = "Initialization",
                Lat = lamp.Property.Address.Lat,
                Lng = lamp.Property.Address.Lng,
                IsOn = lamp.IsOn,
                Regime = lamp.Regime.ToString()
            };

        }

        private SolarPanelInitializationDTO initializeSolarPanel(Device device)
        {
            SolarPanel panel = (SolarPanel)device;
                
            return new SolarPanelInitializationDTO
            {
                Type = "Initialization",
                Efficiency = panel.Efficiency,
                NumberOfPanels = panel.NumberOfPanels,
                Size = panel.Size,
                Lat = panel.Property.Address.Lat,
                Lng = panel.Property.Address.Lng,
                //TODO: da li treba dodati on
            };
        }

        private AmbientSensorInitializationDTO initializeAmbientalSensor(Device device)
        {
            AmbientSensor ambientSensor = (AmbientSensor)device;

            return new AmbientSensorInitializationDTO
            {
                Type = "Initialization",
                Lat = ambientSensor.Property.Address.Lat,
                Lng = ambientSensor.Property.Address.Lng,
            };
        }
    }
}
