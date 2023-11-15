using System.ComponentModel.DataAnnotations;

namespace nvt_back
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
