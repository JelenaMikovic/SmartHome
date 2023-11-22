using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Devices
{
    public class AirConditionerDTO : DeviceDTO
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        public List<AirConditionerMode> SupportedModes { get; set; }

        [Required(ErrorMessage = "Min temperature field is required")]
        [Range(-5, 40, ErrorMessage = "Min temperature should be between -5 and 40")]
        public double MinTemperature { get; set; }

        [Required(ErrorMessage = "Max temperature field is required")]
        [Range(-5, 40, ErrorMessage = "Max temperature should be between -5 and 40")]
        public double MaxTemperature { get; set; }
    }
}
