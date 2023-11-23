using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace nvt_back.DTOs
{
    public class DeviceRegistrationDTO
    {
        [Required (ErrorMessage="Name field is required")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType DeviceType { get; set; }

        public bool IsOnline { get; set; }
        public PowerSource PowerSource { get; set; }

        [Required(ErrorMessage = "Power consumption field is required")]
        [Range(0, 1000, ErrorMessage = "Power consumption should be between 0 and 1000")]
        public double PowerConsumption { get; set; }

        [Required(ErrorMessage = "Image field is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Property id field is required")]
        public int PropertyId { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, DeviceType: {DeviceType}, IsOnline: {IsOnline}, PowerSource: {PowerSource}, PowerConsumption: {PowerConsumption}, Image: {Image}";
        }

    }
}
