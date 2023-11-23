using System.Data;

namespace nvt_back.DTOs
{
    internal class UserDTO
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActivated { get; set; }
        public UserRole Role { get; set; }


        public UserDTO(User user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.Name = user.Name;
            this.Surname = user.Surname;
            this.Role = user.Role;
            this.IsActivated = user.IsActivated;
        }
    }
}