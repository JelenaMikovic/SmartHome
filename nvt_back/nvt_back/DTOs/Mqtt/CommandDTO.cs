using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.Mqtt
{
    public enum CommandAction
    {
        TURN_ON,
        TURN_OFF,
        AUTOMATIC,
        MANUAL
    }

    public class CommandDTO
    {
        [Required(ErrorMessage = "DeviceId field is required")]
        public int DeviceId { get; set; }

        [Required(ErrorMessage = "UserId field is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Action field is required")]
        public CommandAction Action { get; set; }

    }
}
