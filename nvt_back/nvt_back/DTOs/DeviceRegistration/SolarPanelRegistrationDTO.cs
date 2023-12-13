using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class SolarPanelRegistrationDTO : DeviceRegistrationDTO
    {
        [Required(ErrorMessage = "Size field is required")]
        [Range(0, 1000, ErrorMessage = "Size should be between 0 and 1000")]
        public double Size { get; set; }

        [Required(ErrorMessage = "Efficiency field is required")]
        [Range(0, 100, ErrorMessage = "Efficiency should be between 0 and 100")]
        public double Efficiency { get; set; }

        [Required(ErrorMessage = "Number of panels field is required")]
        [Range(1, 20, ErrorMessage = "Number of panels should be between 1 and 20")]
        public int NumberOfPanels { get; set; }
    }
}
