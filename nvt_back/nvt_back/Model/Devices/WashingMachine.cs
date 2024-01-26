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
        public bool IsOn { get; set; }
        public WashingMachineMode CurrentMode { get; set; }
        //public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }

        public WashingMachine() { }

        public WashingMachine(WashingMachineRegistrationDTO dto) : base(dto)
        {
            SupportedModes = dto.SupportedModes;
            IsOn = false;
            DeviceType = DeviceType.WASHING_MACHINE;
        }
    }
}
