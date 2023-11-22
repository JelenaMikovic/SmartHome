using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Devices
{
    public class HomeBatteryDTO : DeviceDTO
    {
        [Required(ErrorMessage = "Capacity field is required")]
        [Range(0, 18, ErrorMessage = "Capacity should be between 0 and 18")]
        public double Capacity { get; set; }

        [Required(ErrorMessage = "Battery health field is required")]
        [Range(0, 18, ErrorMessage = "Battery health should be between 0 and 100")]
        public double Health { get; set; }
        public double? CurrentCharge { get; set; }
    }
}
