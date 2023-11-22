using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public class IrrigationSystem : Device
    {
        [Required(ErrorMessage = "Is on field is required")]
        public bool IsOn { get; set; }
        //public Dictionary<string, TimeSchedule> CustomSchedules { get; set; }
    }
}
