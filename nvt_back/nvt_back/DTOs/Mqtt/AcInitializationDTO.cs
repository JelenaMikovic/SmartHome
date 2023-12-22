using nvt_back.Model.Devices;

namespace nvt_back.DTOs.Mqtt
{
    public class AcInitializationDTO
    {
        public string Type { get; set; }
        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
        public List<string> SupportedModes { get; set; }
        public double CurrentTemperature { get; set; }
        public string CurrentMode { get; set; }
        public bool IsOn { get; set; }
    }
}
