using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class AmbientSensorReportResponseDTO
    {
        public List<dynamic> TemperatureData { get; set; }
        public List<dynamic> HumidityData { get; set; }
    }
}
