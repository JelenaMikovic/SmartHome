using System.ComponentModel.DataAnnotations;

namespace nvt_back
{
    public enum UserRole
    {
        USER,
        ADMIN,
        SUPERADMIN
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActivated { get; set; }
        public UserRole Role { get; set; }
        public List<Property> OwnedProperties { get; set; }
    }
}
