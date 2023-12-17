using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class AmbientSensorDetailsDTO : DeviceDetailsDTO
    {
        public double CurrentTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public int UpdateIntervalSeconds { get; set; }
    }
}
