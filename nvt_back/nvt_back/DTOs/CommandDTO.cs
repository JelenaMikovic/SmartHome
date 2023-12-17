using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs
{
    public class CommandDTO
    {
        [Required(ErrorMessage = "DeviceId field is required")]
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "DeviceType field is required")]
        public String DeviceType { get; set; }

        [Required(ErrorMessage = "Action field is required")]
        public String Action { get; set; }

        [Required(ErrorMessage = "Valur field is required")]
        public String Value { get; set; }

    }
}
