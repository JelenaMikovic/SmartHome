using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Mqtt
{
    public class PlateUpdateDTO
    {
        [Required(ErrorMessage = "DeviceId field is required")]
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "Measurement field is required")]
        public string Measurement { get; set; }

        [Required(ErrorMessage = "Value field is required")]
        public String Value { get; set; }
    }
}
