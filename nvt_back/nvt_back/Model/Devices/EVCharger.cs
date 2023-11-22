using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public class EVCharger : Device
    {
        [Required(ErrorMessage = "Number of ports field is required")]
        [Range(1, 20, ErrorMessage = "Number of ports should be between 1 and 20")]
        public int NumberOfPorts { get; set; }

        [Required(ErrorMessage = "Charging power field is required")]
        [Range(5, 150, ErrorMessage = "Charging power should be between 1 and 20")]
        public double ChargingPower { get; set; }

        [Required(ErrorMessage = "Charging threshold field is required")]
        [Range(0, 100, ErrorMessage = "Charging threshold should be between 0 and 100")]
        public double ChargingThreshold { get; set; }

        /* public int MaxChargingTime { get; set; }
         public List<EVChargingSession> ChargingSessions { get; set; }*/

        public EVCharger() { }
        public EVCharger(EVChargerRegistrationDTO dto)
        {
            Name = dto.Name;
            IsOnline = false;
            PowerConsumption = dto.PowerConsumption;
            PowerSource = dto.PowerSource;
            Image = dto.Image;
            NumberOfPorts = dto.NumberOfPorts;
            ChargingPower = dto.ChargingPower;
            ChargingThreshold = dto.ChargingThreshold;
        }
    }
}
