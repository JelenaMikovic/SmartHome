namespace nvt_back.DTOs.Mqtt
{
    public class VehicleGateInitializationDTO
    {
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsOpen { get; set; }
        public bool IsPrivate { get; internal set; }
        public List<string> AllowedLicencePlates { get; set; }
    }
}
