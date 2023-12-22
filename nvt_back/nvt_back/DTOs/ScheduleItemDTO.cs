using System.Data;

namespace nvt_back.DTOs
{
    public class ScheduleItemDTO
    {
        public int ScheduleId { get; set; }
        public int DeviceId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Mode { get; set; }
        public double Temperature { get; set; }

        public ScheduleItemDTO() { }

    }
}