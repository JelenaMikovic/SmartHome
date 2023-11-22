namespace nvt_back.Model.Devices
{
    public class HomeBattery : Device
    {
        public double Capacity { get; set; }
        public double Health { get; set; }
        public double CurrentCharge { get; set; }
        /*public double PowerConsumption { get; set; }
        public double PowerProduction { get; set; }*/

    }
}
