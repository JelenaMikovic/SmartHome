using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model
{
    public class ActivationCode
    {

        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}
