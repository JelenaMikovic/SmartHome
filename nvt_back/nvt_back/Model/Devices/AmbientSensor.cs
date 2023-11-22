namespace nvt_back.Model.Devices
{
    public class AmbientSensor : Device
    {
        public double CurrentTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public int UpdateIntervalSeconds { get; set; }
    }
}
