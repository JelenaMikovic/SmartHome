namespace nvt_back.Model.Devices
{
    public class EVCharger : Device
    {
        public int NumberOfPorts { get; set; }
        public double ChargingPower { get; set; }
        public double ChargingThreshold { get; set; }

        /* public int MaxChargingTime { get; set; }
         public List<EVChargingSession> ChargingSessions { get; set; }*/
    }
}
