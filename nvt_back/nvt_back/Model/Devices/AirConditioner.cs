using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class AirConditioner : Device
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
        /*public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }*/

        public AirConditioner() { }
        public AirConditioner(AirConditionerRegistrationDTO dto) : base(dto)
        {
            //Image = dto.Image;
            SupportedModes = dto.SupportedModes;
            MinTemperature = dto.MinTemperature;
            MaxTemperature = dto.MaxTemperature;
            DeviceType = DeviceType.AC;
        }
    }
}
