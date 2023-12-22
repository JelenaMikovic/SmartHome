using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class AirConditionerSchedule
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public AirConditionerMode Mode { get; set; }
        public double Temperature { get; set; }

        public AirConditionerSchedule() { }

        public static implicit operator AirConditionerSchedule(EntityEntry<AirConditionerSchedule> v)
        {
            throw new NotImplementedException();
        }
    }
}
