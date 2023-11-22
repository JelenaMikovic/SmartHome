namespace nvt_back.Model.Devices
{
    public enum AirConditionerMode {
        COOLING, 
        HEATING,
        AUTOMATIC,
        VENTILATION,
    }
    public class AirConditioner : Device
    {
        public List<AirConditionerMode> SupportedModes { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        /*public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }*/
    }
}
