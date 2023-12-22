using nvt_back.DTOs.DeviceDetailsDTO;
using nvt_back.Model.Devices;

namespace nvt_back.Repositories
{
    internal class ACDetailsDTO : DeviceDetailsDTO
    {
        public bool IsOn { get; set; }
        public string CurrentMode { get; set; }
        public double CurrentTemperature { get; set; }
        public List<String> SupportedModes { get; set; }
        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
    }
}