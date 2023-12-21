namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class VehicleGateDetailsDTO : DeviceDetailsDTO
    {
        public bool IsPrivate { get; set; }
        public bool IsOpen { get; set; }
        public List<string> AllowedLicencePlates { get; set; }
    }
}
