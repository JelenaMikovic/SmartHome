namespace nvt_back.DTOs.DeviceDetailsDTO
{
    public class LampDetailsDTO : DeviceDetailsDTO
    {
        public bool IsOn { get; set; }
        public int BrightnessLevel { get; set; }
        public String Regime { get; set; }
    }
}
