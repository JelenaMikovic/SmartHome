using nvt_back.DTOs;
using nvt_back.DTOs.DeviceRegistration;
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

        public void Add(DeviceRegistrationDTO dto)
        {
            Console.WriteLine(dto.ToString());
            Device deviceForDb = null;
            if (dto is AirConditionerRegistrationDTO)
                deviceForDb = new AirConditioner((AirConditionerRegistrationDTO)dto);
            else if (dto is AmbientSensorRegistrationDTO)
                deviceForDb = new AmbientSensor((AmbientSensorRegistrationDTO)dto);
            else if (dto is EVChargerRegistrationDTO)
                deviceForDb = new EVCharger((EVChargerRegistrationDTO)dto);
            else if (dto is HomeBatteryRegistrationDTO)
                deviceForDb = new HomeBattery((HomeBatteryRegistrationDTO)dto);
            else if (dto is IrrigationSystemRegistrationDTO)
                deviceForDb = new IrrigationSystem((IrrigationSystemRegistrationDTO)dto);
            else if (dto is LampRegistrationDTO)
                deviceForDb = new Lamp((LampRegistrationDTO)dto);
            else if (dto is SolarPanelRegistrationDTO)
                deviceForDb = new SolarPanel((SolarPanelRegistrationDTO)dto);
            else if (dto is VehicleGateRegistrationDTO)
                deviceForDb = new VehicleGate((VehicleGateRegistrationDTO)dto);
            else if (dto is WashingMachineRegistrationDTO)
                deviceForDb = new WashingMachine((WashingMachineRegistrationDTO)dto);

            this._deviceRegistrationRepository.Add(deviceForDb);
        }
    }
}
