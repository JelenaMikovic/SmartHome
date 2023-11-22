using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs;

namespace nvt_back.Model.Devices
{
    public class Lamp : Device
    {
        public bool IsOn { get; set; }
        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public int BrightnessLevel { get; set; }

        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LampColor Color { get; set; }

        public Lamp() { }
        public Lamp(LampRegistrationDTO dto)
        {
            Name = dto.Name;
            IsOnline = false;
            PowerConsumption = dto.PowerConsumption;
            PowerSource = dto.PowerSource;
            IsOn = dto.IsOn;
            BrightnessLevel = dto.BrightnessLevel;
            Color = dto.Color;
            Image = dto.Image;
            DeviceType = DeviceType.LAMP;
        }
    }
}
