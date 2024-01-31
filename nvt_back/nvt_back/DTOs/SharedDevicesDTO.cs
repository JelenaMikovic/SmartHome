using nvt_back.Model.Devices;

namespace nvt_back.DTOs
{
    public class SharedDeviceDTO
    {
        public string Email { get; set; }
        public int DeviceId { get; set; }
        public int PropertyId { get; set; }
        public string SharedType { get; set; }
    }
}