using nvt_back.Model.Devices;

namespace nvt_back.DTOs
{
    public class DeviceDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public PowerSource PowerSource { get; set; }
        public double PowerConsumption { get; set; }
        public string Image { get; set; }
        
    }
}
