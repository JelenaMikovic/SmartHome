namespace nvt_back.Model.Devices
{
    public class VehicleGate : Device
    {
        public bool IsPrivateModeOn { get; set; }
        public List<string> AllowedLicencePlates { get; set; }
        public bool IsOpened { get; set; }
    }
}
