using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class SolarPanelDetailsDTO : DeviceDetailsDTO
    {
        public bool IsOn { get; set; }
        public double Size { get; set; }
        public double Efficiency { get; set; }
        public int NumberOfPanels { get; set; }
    }
}
