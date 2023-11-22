using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs
{
    public class DeviceDTO
    {
        [Required (ErrorMessage="Name field is required")]
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public PowerSource PowerSource { get; set; }

        [Required(ErrorMessage = "Power consumption field is required")]
        [Range(0, 1000, ErrorMessage = "Power consumption should be between 0 and 1000")]
        public double PowerConsumption { get; set; }

        [Required(ErrorMessage = "Image field is required")]
        public string Image { get; set; }
    }
}
