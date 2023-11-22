using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Devices
{
    public class EVChargerDTO : DeviceDTO
    {
        [Required(ErrorMessage = "Number of ports field is required")]
        [Range(1, 20, ErrorMessage = "Number of ports should be between 1 and 20")]
        public int NumberOfPorts { get; set; }

        [Required(ErrorMessage = "Charging power field is required")]
        [Range(5, 150, ErrorMessage = "Charging power should be between 1 and 20")]
        public double ChargingPower { get; set; }

        [Required(ErrorMessage = "Charging threshold field is required")]
        [Range(0, 100, ErrorMessage = "Charging threshold should be between 0 and 100")]
        public double ChargingThreshold { get; set; }
    }
}
