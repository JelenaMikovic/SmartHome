using nvt_back.DTOs.DeviceRegistration;
using nvt_back.InfluxDB;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly InfluxDBService _influxDBService;
        public DeviceService(IDeviceRepository deviceRepository, InfluxDBService influxDBService)
        {
            _deviceRepository = deviceRepository;
            _influxDBService = influxDBService;
        }

        public async Task<List<Device>> GetAll()
        {
            return await _deviceRepository.GetAll();
        }

        public async Task<IEnumerable<dynamic>> GetBrightnessLevelData(ReportDTO dto)
        {
            try
            {
                string query = $"from(bucket: \"measurements\")" +
                               $" |> range(start: -7d)" +
                               $" |> filter(fn: (r) => r[\"_measurement\"] == \"illuminance\" and r[\"_field\"] == \"illuminance\" and r[\"device_id\"] == \"{dto.DeviceId}\")";

                Console.WriteLine(query);
                var result = await _influxDBService.QueryAsync(query);
                var processedData = result
                    .Select(record => new
                    {
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.GetTime()?.ToUnixTimeMilliseconds() ?? 0)
                          .UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Value = record.GetValueByIndex(5)
                    });

                return processedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetEnvironmentalData(ReportDTO dto, string field)
        {
            try
            {
                string query = $"from(bucket: \"measurements\")" +
                               $" |> range(start: -7d)" +
                               $" |> filter(fn: (r) => r[\"_measurement\"] == \"ambiental_sensor\" and r[\"_field\"] == \"{field}\" and r[\"device_id\"] == \"{dto.DeviceId}\")";

                Console.WriteLine(query);
                var result = await _influxDBService.QueryAsync(query);
                var processedData = result
                    .Select(record => new
                    {
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.GetTime()?.ToUnixTimeMilliseconds() ?? 0)
                          .UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Value = record.GetValueByIndex(5)
                    });

                return processedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<int>> GetPropertyIdsWithHomeBatteries()
        {
            return await _deviceRepository.GetPropertyIdsWithHomeBatteries();
        }

        public async Task<List<HomeBattery>> GetAllBatteriesForPropertyId(int propertyId)
        {
            return await _deviceRepository.GetAllBatteriesForPropertyId(propertyId);
        }

        public async Task<List<Device>> GetConsumingPowerDevicesForProperty(int propertyId)
        {
            return await _deviceRepository.GetConsumingPowerDevicesForProperty(propertyId);
        }
    }
}
