using System.ComponentModel.DataAnnotations;

namespace nvt_back.Model.Devices
{
    public enum PowerSource
    {
        ALKALINE_BATTERY,
        HOUSE_BATTERY,
        ELETRICAL_GRID
    }

    public abstract class Device
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public PowerSource PowerSource { get; set; }
        public double PowerConsumption { get; set; }
    }
}
