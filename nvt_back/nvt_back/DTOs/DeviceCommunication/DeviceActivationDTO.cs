using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceCommunication
{
    public class DeviceActivationDTO
    {
        [Required(ErrorMessage = "Id field is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Activate field is required")]
        public bool Activate { get; set; }
    }
}
