using System.ComponentModel.DataAnnotations;
using System.Data;

namespace nvt_back.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Role { get; set; }
        //[Required]
        //public string Image { get; set; }

    }
}
