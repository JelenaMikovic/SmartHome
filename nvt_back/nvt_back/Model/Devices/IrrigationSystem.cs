using System.ComponentModel.DataAnnotations;
using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class IrrigationSystem : Device
    {
        [Required(ErrorMessage = "Is on field is required")]
        public bool IsOn { get; set; }
        //public Dictionary<string; TimeSchedule> CustomSchedules { get; set; }

        public IrrigationSystem() { }
        public IrrigationSystem(IrrigationSystemRegistrationDTO dto) : base(dto)
        {
            IsOn = false;
            DeviceType = DeviceType.IRRIGATION_SYSTEM;
        }
    }
}
