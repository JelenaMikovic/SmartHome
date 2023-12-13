using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace nvt_back.Mqtt
{
    public enum Status
    {
        OFF,
        ON
    }

    public enum Sender
    {
        PLATFORM,
        DEVICE
    }

    public class Heartbeat
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Status? Status { get; set; }
        public string? PlatformResponse { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Sender Sender { get; set; }
        public int DeviceId { get; set; }

        public bool InitializeParameters { get; set; }
    }
}
