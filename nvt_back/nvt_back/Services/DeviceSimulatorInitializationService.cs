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
            if (device.DeviceType == DeviceType.VEHICLE_GATE)
                return initializeVehicleGate(device);
            if (device.DeviceType == DeviceType.HOME_BATTERY) 
                return initializeBattery(device);

            return null;
        }

        private LampInitializationDTO initializeLamp(Device device)
        {
            Lamp lamp = (Lamp)device;

            return new LampInitializationDTO
            {
                Type = "Initialization",
                Lat = lamp.Property.Address.Lat,
                Lng = lamp.Property.Address.Lng,
                IsOn = lamp.IsOn,
                Regime = lamp.Regime.ToString(),
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
                PropertyId = panel.PropertyId,
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
        private VehicleGateInitializationDTO initializeVehicleGate(Device device)
        {
            VehicleGate gate = (VehicleGate)device;

            return new VehicleGateInitializationDTO
            {
                Type = "Initialization",
                Lat = gate.Property.Address.Lat,
                Lng = gate.Property.Address.Lng,
                IsOpen = gate.IsOpened,
                IsPrivate = gate.IsPrivateModeOn,
                AllowedLicencePlates = gate.AllowedLicencePlates
            };
        }


        private BatteryInitializationDTO initializeBattery(Device device)
        {
            HomeBattery battery = (HomeBattery)device;

            return new BatteryInitializationDTO
            {
                Capacity = battery.Capacity,
                CurrentCharge = battery.CurrentCharge,
            };
        }
    }
}
