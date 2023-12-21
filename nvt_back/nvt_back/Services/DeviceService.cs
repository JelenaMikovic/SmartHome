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
        public DeviceService(IDeviceRepository deviceRepository, InfluxDBService influxDBService, IUserRepository userRepository)
        {
            _deviceRepository = deviceRepository;
            _influxDBService = influxDBService;
            _userRepository = userRepository;
        }

        public async Task<List<Device>> GetAll()
        {
            return await _deviceRepository.GetAll();
        }

        public async Task<IEnumerable<dynamic>> GetActionTableData(int id)
        {
            try
            {
                string query = $"from(bucket: \"measurements\")" +
                               $" |> range(start: 0)" +
                               $" |> filter(fn: (r) => r[\"_measurement\"] == \"command\")" +
                               $" |> filter(fn: (r) => r[\"_field\"] == \"type\" or r[\"_field\"] == \"time\" or r[\"_field\"] == \"value\")" +
                               $" |> filter(fn: (r) => r[\"device_id\"] == \"{id}\" and r[\"success\"] == \"True\")";

                var result = await _influxDBService.QueryAsync(query);
                var tasks = result.Select(async record =>
                {
                    string userIdO = (string)record.GetValueByKey("user");
                    int userId = Int32.Parse(userIdO);
                    User user = await _userRepository.GetById(userId);
                    return new
                    {
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.GetTime()?.ToUnixTimeMilliseconds() ?? 0)
                                .UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        User = user.Name + " " + user.Surname,
                        Action = record.GetValueByKey("type"),
                        State = record.GetValue()
                    };
                    /*if (userIdO is int userId)
                    {
                        User user = await _userRepository.GetById(userId);

                        return new
                        {
                            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.GetTime()?.ToUnixTimeMilliseconds() ?? 0)
                                .UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            User = user.Name + " " + user.Surname,
                            Action = record.GetValueByKey("type"),
                            State = record.GetValue()
                        };
                    }*/
                    return null;
                });

                var processedData = await Task.WhenAll(tasks);
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
