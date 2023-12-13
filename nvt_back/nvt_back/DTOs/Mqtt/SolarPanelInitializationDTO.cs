namespace nvt_back.DTOs.Mqtt
{
    public class SolarPanelInitializationDTO
    {
        public string Type { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int NumberOfPanels { get; set; }
        public double Efficiency { get; set; }
        public double Size { get; set; }
    }
}
