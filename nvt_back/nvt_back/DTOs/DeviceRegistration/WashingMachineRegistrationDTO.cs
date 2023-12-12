using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class WashingMachineRegistrationDTO : DeviceRegistrationDTO
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public List<WashingMachineMode> SupportedModes { get; set; }
    }
}
