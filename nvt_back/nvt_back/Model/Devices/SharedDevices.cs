using nvt_back.DTOs.DeviceRegistration;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs;

namespace nvt_back.Model.Devices
{
    public enum SharedType
    {
        PROPERTY,
        DEVICE
    }

    public enum SharedStatus
    {
        ACCEPTED,
        DENIED,
        PENDING
    }

    public class SharedDevices
    {

        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int? DeviceId { get; set; }
        public int? PropertyId { get; set; }
        public SharedStatus Status { get; set; }
        public SharedType SharedType { get; set; }

        public SharedDevices() { }
    }
}
