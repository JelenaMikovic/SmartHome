namespace nvt_back.DTOs.Mqtt
{
    public class BatteryInitializationDTO
    {
        public int Id { get; set; }
        public double Capacity { get; set; }
        public double CurrentCharge { get; set; }
    }
}
