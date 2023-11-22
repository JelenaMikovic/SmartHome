using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Devices
{
    public class WashingMachineDTO : DeviceDTO
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        public List<WashingMachineMode> SupportedModes { get; set; }
    }
}
