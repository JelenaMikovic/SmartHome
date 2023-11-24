using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using nvt_back.DTOs;

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

        //[ForeignKey("Property")]
        [Required(ErrorMessage = "Property id field is required")]
        public int PropertyId { get; set; }

        //public virtual Property Property { get; set; }

        public Device() { }
        public Device(DeviceRegistrationDTO dto)
        {
            Name = dto.Name;
            IsOnline = false;
            PowerConsumption = dto.PowerConsumption;
            PowerSource = dto.PowerSource;
            PropertyId = dto.PropertyId;
        }
    }
}
