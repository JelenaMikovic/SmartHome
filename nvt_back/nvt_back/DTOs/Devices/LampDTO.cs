using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs
{
    public class LampDTO : DeviceDTO
    {
        public bool IsOn { get; set; }

        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public int BrightnessLevel { get; set; }

        [Required (ErrorMessage="Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public LampColor Color { get; set; }
    }
}
