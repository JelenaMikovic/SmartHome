using InfluxDB.Client.Api.Domain;
using nvt_back.DTOs.DeviceRegistration;
using nvt_back.InfluxDB;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using System.Threading.Tasks;

namespace nvt_back.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly InfluxDBService _influxDBService;
        private readonly IUserRepository _userRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        public DeviceService(IDeviceRepository deviceRepository, InfluxDBService influxDBService, IUserRepository userRepository, IServiceScopeFactory scopeFactory)
        {
            _deviceRepository = deviceRepository;
            _influxDBService = influxDBService;
            _userRepository = userRepository;
            _scopeFactory = scopeFactory;
        }

        public async Task<List<Device>> GetAll()
        {
            return await _deviceRepository.GetAll();
        }

        public async Task<IEnumerable<dynamic>> GetActionTableData(int id)
        {
            try
            {
                string query = $"from(bucket: \"commands\")" +
               $" |> range(start: 0)" +
               $" |> filter(fn: (r) => r[\"_measurement\"] == \"command\")" +
               $" |> filter(fn: (r) => r[\"_field\"] == \"type\" or r[\"_field\"] == \"time\" or r[\"_field\"] == \"value\")" +
               $" |> filter(fn: (r) => r[\"device_id\"] == \"{id}\" and r[\"success\"] == \"True\")" +
               $" |> sort(columns: [\"_time\"], desc: true)";

                var result = await _influxDBService.QueryAsync(query);
                var processedData = result
                    .Select(record => new
                    {
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.GetTime()?.ToUnixTimeMilliseconds() ?? 0)
                                .UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        User = record.GetValueByIndex(14),
                        Action = record.GetValueByIndex(13),
                        State = record.GetValueByIndex(5)
                    });

                return processedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetBrightnessLevelData(ReportDTO dto)
        {
            try
            {
                string query = $"from(bucket: \"measurements\")" +
                               $" |> range(start: {dto.StartDate}, stop: {dto.EndDate})" +
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
                               $" |> range(start: {dto.StartDate}, stop: {dto.EndDate})" +
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
