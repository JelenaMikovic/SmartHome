using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public class HomeBattery : Device
    {
        [Required(ErrorMessage = "Capacity field is required")]
        [Range(0, 18, ErrorMessage = "Capacity should be between 0 and 18")]
        public double Capacity { get; set; }

        [Required(ErrorMessage = "Battery health field is required")]
        [Range(0, 100, ErrorMessage = "Battery health should be between 0 and 100")]
        public double Health { get; set; }

        [Required(ErrorMessage = "Current charge field is required")]
        [Range(0, 100, ErrorMessage = "Current charge should be between 0 and 100")]
        public double CurrentCharge { get; set; }
        /*public double PowerConsumption { get; set; }
        public double PowerProduction { get; set; }*/

        public HomeBattery() { }

        public HomeBattery(HomeBatteryRegistrationDTO dto) : base(dto)
        {
            Capacity = dto.Capacity;
            Health = dto.Health;
            CurrentCharge = 100;
            DeviceType = DeviceType.HOME_BATTERY;
        }
    }
}
