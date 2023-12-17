using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class AmbientSensorReportDTO
    {
        public int DeviceId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
