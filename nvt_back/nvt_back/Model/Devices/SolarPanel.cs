using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public class SolarPanel : Device
    {
        [Required(ErrorMessage = "Size field is required")]
        [Range(0, 1000, ErrorMessage = "Size should be between 0 and 1000")]
        public double Size { get; set; }

        [Required(ErrorMessage = "Efficiency field is required")]
        [Range(0, 100, ErrorMessage = "Efficiency should be between 0 and 100")]
        public double Efficiency { get; set; }

        [Required(ErrorMessage = "Is on field is required")]
        public bool IsOn { get; set; }

        public SolarPanel() { }
        public SolarPanel(SolarPanelRegistrationDTO dto) : base(dto)
        { 
            Size = dto.Size;
            Efficiency = dto.Efficiency;
            IsOn = false;
            DeviceType = DeviceType.SOLAR_PANEL;
        }

    }
}
