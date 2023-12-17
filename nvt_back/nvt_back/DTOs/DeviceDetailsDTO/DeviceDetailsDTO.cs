using nvt_back.Model.Devices;

namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class DeviceDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public PowerSource PowerSource { get; set; }
        public double PowerConsumption { get; set; }
        public string Image { get; set; }

        public string DeviceType { get; set; }
    }
}
