using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace nvt_back.Model.Devices
{
    public abstract class Device
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Device type field is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType DeviceType { get; set; }
        public bool IsOnline { get; set; }

        [Required(ErrorMessage = "Power consumption field is required")]
        [Range(0, 100, ErrorMessage = "Power consumption should be between 0 and 1000")]
        public PowerSource PowerSource { get; set; }
        public double PowerConsumption { get; set; }

        [Required(ErrorMessage = "Image field is required")]
        public string Image { get; set; }
    }
}
