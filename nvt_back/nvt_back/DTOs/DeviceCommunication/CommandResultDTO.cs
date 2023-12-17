using nvt_back.DTOs.Mqtt;
using System.ComponentModel.DataAnnotations;

namespace nvt_back.DTOs.DeviceCommunication
{
    public enum CommandResult
    {
        SUCCESS,
        FAILIURE
    }

    public class CommandResultDTO
    {
        [Required(ErrorMessage = "DeviceId field is required")]
        public int DeviceId { get; set; }

        //[Required(ErrorMessage = "UserId field is required")]
        //public int UserId { get; set; }

        [Required(ErrorMessage = "Action field is required")]
        public String Action { get; set; }

        [Required(ErrorMessage = "Value field is required")]
        public String Value { get; set; }

        //[Required(ErrorMessage = "Timestamp field is required")]
        //public DateTime Timestamp{ get; set; }

        [Required(ErrorMessage = "Result field is required")]
        public CommandResult Result { get; set; }

        [Required(ErrorMessage = "Message field is required")]
        public String Message { get; set; }
    }
}
