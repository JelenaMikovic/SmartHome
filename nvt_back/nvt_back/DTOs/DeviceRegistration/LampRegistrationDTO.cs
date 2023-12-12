using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace nvt_back.DTOs
{
    public class LampRegistrationDTO : DeviceRegistrationDTO
    {
        public bool IsOn { get; set; }

        [Required(ErrorMessage = "Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        public int BrightnessLevel { get; set; }

        [Required (ErrorMessage="Color field is required")]
        [Range(0, 100, ErrorMessage = "Brightness should be between 0 and 100")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LampColor Color { get; set; }
    }
}
