using nvt_back.Model.Devices;

namespace nvt_back.DTOs.Mqtt
{
    public class WashingMachineInitializationDTO
    {
        public string Type { get; set; }
        public List<string> SupportedModes { get; set; }
        public string CurrentMode { get; set; }
        public bool IsOn { get; set; }
    }
}
