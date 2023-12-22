using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using Quartz;

namespace nvt_back
{
    public class Job : IJob
    {
        private readonly IMqttClientService _mqttClientService;

        public Job(IMqttClientService mqttClientService)
        {
            _mqttClientService = mqttClientService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string action = context.JobDetail.JobDataMap.GetString("Action");
                int deviceId = int.Parse(context.JobDetail.JobDataMap.GetString("DeviceId"));

                if (action == "On")
                {
                    _mqttClientService.PublishStatusUpdate(deviceId, "On", 0);
                    string mode = context.JobDetail.JobDataMap.GetString("Mode");
                    _mqttClientService.PublishModeUpdate(deviceId, mode, 0);
                    string temperature = context.JobDetail.JobDataMap.GetString("Temperature");
                    _mqttClientService.PublishTemperatureUpdate(deviceId, temperature, 0);
                }
                else if (action == "Off")
                {
                    _mqttClientService.PublishStatusUpdate(deviceId, "Off", 0);
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Internal Server Error: " + ex.Message);
            }
        }
    }
}