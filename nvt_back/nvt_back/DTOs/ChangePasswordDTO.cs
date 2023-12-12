using System.Data;

namespace nvt_back.DTOs
{
    public class ChangePasswordDTO
    {

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public ChangePasswordDTO() { }
    }
}