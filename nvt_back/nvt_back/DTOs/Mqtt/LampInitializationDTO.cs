namespace nvt_back.DTOs.Mqtt
{
    public class LampInitializationDTO
    {
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Regime { get; set; }
    }
}
