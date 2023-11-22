namespace nvt_back.Model.Devices
{
    public enum WashingMachineMode
    {
        COTTON,
        SPORTSWEAR,
        IRON_DRY,
        TOWELS,
        MIX,
        WOOL,
        SUPER30,
        CUPBOARD
    }
    public class WashingMachine : Device
    {
        public List<WashingMachineMode> SupportedModes { get; set; }
        //public Dictionary<string, TemperatureSchedule> CustomSchedules { get; set; }
    }
}
