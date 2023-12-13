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
            if (device == null)
            {
                throw new ArgumentException("err");
            }
            if (device.DeviceType == DeviceType.SOLAR_PANEL)
                return initializeSolarPanel(device);
            return null;
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
            };
        }
    }
}
