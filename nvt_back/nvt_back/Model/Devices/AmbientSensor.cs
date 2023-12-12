using nvt_back.DTOs.DeviceRegistration;

namespace nvt_back.Model.Devices
{
    public class AmbientSensor : Device
    {
        public double CurrentTemperature { get; set; }
        public double CurrentHumidity { get; set; }
        public int UpdateIntervalSeconds { get; set; }

        public AmbientSensor() { }
        public AmbientSensor(AmbientSensorRegistrationDTO dto) : base(dto)
        {
            //Image = dto.Image;
            DeviceType = DeviceType.AMBIENT_SENSOR;
        }
    }
}
