using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs;

namespace nvt_back.Model.Devices
{
    public enum LampRegime
    {
        MANUAL,
        AUTOMATIC
    }

    public class Lamp : Device
    {
        public bool IsOn { get; set; }

        [Required(ErrorMessage = "Brightness field is required")]
        [Range(0, 100000, ErrorMessage = "Brightness should be between 0 and 100.000")]
        public int BrightnessLevel { get; set; }

        [Required(ErrorMessage = "Regime field is required")]
        public LampRegime Regime { get; set; }

        public Lamp() { }
        public Lamp(LampRegistrationDTO dto)
        {
            IsOn = dto.IsOn;
            BrightnessLevel = dto.BrightnessLevel;
            DeviceType = DeviceType.LAMP;
            Regime = LampRegime.MANUAL;
        }
    }
}
