namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class BatteryDetailsDTO : DeviceDetailsDTO
    {
        public int Id { get; set; }
        public double CurrentCharge { get; set; }
        public double Capacity { get; set; }
        public int PropertyId { get; set; }
    }
}
