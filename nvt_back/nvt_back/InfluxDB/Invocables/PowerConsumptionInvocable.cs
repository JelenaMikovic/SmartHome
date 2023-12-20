using Coravel.Invocable;
using Newtonsoft.Json;
using nvt_back.DTOs.Mqtt;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Services.Interfaces;

namespace nvt_back.InfluxDB.Invocables
{
    public class PowerConsumptionInvocable : IInvocable
    {
        private readonly InfluxDBService _influxDBService;
        private readonly IDeviceOnlineStatusService _deviceOnlineStatusService;
        private readonly IDeviceService _deviceService;
        private readonly IMqttClientService _mqttClientService;

        public PowerConsumptionInvocable(InfluxDBService influxDBService,
            IDeviceOnlineStatusService deviceOnlineStatusService,
            IDeviceService deviceService,
            IMqttClientService mqttClientService)
        {
            _influxDBService = influxDBService;
            _deviceOnlineStatusService = deviceOnlineStatusService;
            _deviceService = deviceService;
            _mqttClientService = mqttClientService;
        }


        public async Task Invoke()
        {
            Console.WriteLine("usao");
                List<int> propertyIds = await _deviceService.GetPropertyIdsWithHomeBatteries();
                foreach (var id in propertyIds)
                {
                    List<Device> devices = await _deviceService.GetConsumingPowerDevicesForProperty(id);
                    double powerConsumptionSumPerMinute = devices.Sum(device => device.PowerConsumption) / 60;
                    double gainedBySolarPanel = 0.0;
                    try
                    {
                        string query = $"from(bucket: \"measurements\")" +
                                       $" |> range(start: -1m)" +
                                       $" |> filter(fn: (r) => r[\"_measurement\"] == \"solar_energy\" and r[\"property_id\"] == \"{id}\")" +
                                       $" |> filter(fn: (r) => r[\"_field\"] == \"energy\")" +
                                        $" |> sum(column: \"_value\")";

                        var result = await _influxDBService.QueryAsync(query);
                        Console.WriteLine(result.Count());
                        //var processedData = result.Select(record => gainedBySolarPanel += (double)record.GetValueByKey("_value"));
                        //var processedData = result
                          //  .Select(record => Console.WriteLine((double)record.GetValueByKey("_value")));
                        gainedBySolarPanel = result.Aggregate(0.0, (sum, record) => sum + Convert.ToDouble(record.GetValueByKey("_value")));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        throw;
                    }
                    var payload = new BatteryConsumptionDTO
                    {
                        Type = "Consumption",
                        Consumed = powerConsumptionSumPerMinute,
                        Generated = gainedBySolarPanel
                    };
                    await _mqttClientService.Publish(
                        _mqttClientService.GetHomeBatteryConsumptionTopicForProperty(id),
                        JsonConvert.SerializeObject(payload));
                    Console.WriteLine(gainedBySolarPanel);
                }

                Console.WriteLine("izaso");
            
        }
    }
}
