namespace nvt_back.Model.Devices
{
    public enum LampColor
    {
        WHITE,
        YELLOW,
        GREEN,
        BLUE,
        RED
    }

    public class Lamp : Device
    {
        public bool IsOn { get; set; }
        public int BrightnessLevel { get; set; }
        public LampColor Color { get; set; }
    }
}
