using nvt_back.Model.Devices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nvt_back
{
    public enum PropertyStatus
    {
        PENDING,
        ACCEPTED,
        DENIED
    }

    public class Property
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumOfFloors { get; set; }
        public double Area { get; set; }
        public PropertyStatus Status { get; set; }
        public string ImagePath { get; set; }
        
        [ForeignKey("Owner")]
        public int UserId { get; set; }

        public User Owner { get; set; }

        public List<Device> Devices { get; set; }

    }
}
