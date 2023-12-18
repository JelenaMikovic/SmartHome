using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceRegistration
{
    public class ReportDTO
    {
        public int DeviceId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
