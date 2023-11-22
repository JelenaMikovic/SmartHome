namespace nvt_back.Model.Devices
{
    public class SolarPanel : Device
    {
        public double Size { get; set; }
        public double Efficiency { get; set; }
        public bool IsOn { get; set; }
    }
}
