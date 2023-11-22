using System.ComponentModel.DataAnnotations;
namespace nvt_back.Model.Devices
{
    public class WashingMachine : Device
    {
        [Required(ErrorMessage = "Supported modes field is required")]
        public List<WashingMachineMode> SupportedModes { get; set; }
        //public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }
    }
}
