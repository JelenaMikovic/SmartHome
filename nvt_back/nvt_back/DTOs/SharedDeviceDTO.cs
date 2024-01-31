using nvt_back.Model.Devices;

namespace nvt_back.DTOs
{
    public class SharedDevicesDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? DeviceName { get; set; }
        public string? PropertyName { get; set; }
        public string SharedType { get; set; }
        public string SharedStatus { get; set; }
    }
}