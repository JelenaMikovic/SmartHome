using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class VehicleGate : Device
    {
        public bool IsPrivateModeOn { get; set; }
        public List<string> AllowedLicencePlates { get; set; }
        public bool IsOpened { get; set; }

        public VehicleGate() { }

        public VehicleGate(VehicleGateRegistrationDTO dto)
        {
            IsPrivateModeOn = false;
            AllowedLicencePlates = new List<string>();
            IsOpened = false;
            DeviceType = DeviceType.VEHICLE_GATE;
        }
    }
}
