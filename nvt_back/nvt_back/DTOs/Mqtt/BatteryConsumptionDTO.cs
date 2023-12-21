namespace nvt_back.DTOs.Mqtt
{
    public class BatteryConsumptionDTO
    {
        public string Type {  get; set; }
        public double Consumed { get; set; }
        public double Generated { get; set; }
    }
}
