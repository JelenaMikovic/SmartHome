using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class AirConditionerRegistrationDTO : DeviceRegistrationDTO
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public List<AirConditionerMode> SupportedModes { get; set; }

        [Required(ErrorMessage = "Min temperature field is required")]
        [Range(-5, 40, ErrorMessage = "Min temperature should be between -5 and 40")]
        public double MinTemperature { get; set; }

        [Required(ErrorMessage = "Max temperature field is required")]
        [Range(-5, 40, ErrorMessage = "Max temperature should be between -5 and 40")]
        public double MaxTemperature { get; set; }
    }
}
