using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class WashingMachine : Device
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public List<WashingMachineMode> SupportedModes { get; set; }
        //public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }

        public WashingMachine() { }

        public WashingMachine(WashingMachineRegistrationDTO dto)
        {
            Name = dto.Name;
            IsOnline = false;
            PowerConsumption = dto.PowerConsumption;
            PowerSource = dto.PowerSource;
            Image = dto.Image;
            SupportedModes = dto.SupportedModes;
        }
    }
}
