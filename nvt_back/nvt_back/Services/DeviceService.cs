using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.DependencyInjection;
using nvt_back.DTOs;
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
        private readonly IPropertyRepository _propertyRepository;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeviceService(IDeviceRepository deviceRepository, InfluxDBService influxDBService, IUserRepository userRepository, IServiceScopeFactory scopeFactory, IPropertyRepository propertyService)
        {
            _deviceRepository = deviceRepository;
            _influxDBService = influxDBService;
            _userRepository = userRepository;
            _scopeFactory = scopeFactory;
            _propertyRepository = propertyService;
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
        public async Task<IEnumerable<dynamic>> GetPowerConsumption(ReportDTO dto)
        {
            try
            {
                Console.WriteLine(dto.DeviceId);
               
                string query = $"from(bucket: \"measurements\")" +
                               $" |> range(start: {dto.StartDate}, stop: {dto.EndDate})" +
                               $" |> filter(fn: (r) => r[\"_measurement\"] == \"home_battery\" and r[\"_field\"] == \"consumed_power\" and r[\"property_id\"] == \"{dto.DeviceId}\")";

                var result = await _influxDBService.QueryAsync(query);
                Console.WriteLine(result.Count());
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

        public void AddSchedule(ScheduleItemDTO scheduleItem)
        {
            AirConditionerSchedule schedule = new AirConditionerSchedule();
            schedule.DeviceId = scheduleItem.DeviceId;
            schedule.Mode = Enum.Parse<AirConditionerMode>(scheduleItem.Mode, true);
            schedule.Temperature = scheduleItem.Temperature;
            schedule.EndTime = scheduleItem.EndTime;
            schedule.StartTime = scheduleItem.StartTime;
            _deviceRepository.AddSchedule(schedule);
            return;
        }

        public void RemoveSchedule(int scheduleId)
        {
            _deviceRepository.RemoveSchedule(scheduleId);
        }

        public List<AirConditionerSchedule> GetDeviceSchedule(int deviceId)
        {
            return _deviceRepository.GetDeviceSchedules(deviceId).Result;
        }

        public async void AddSharedDevice(SharedDeviceDTO dto, int id)
        {
            User user = _userRepository.GetByEmail(dto.Email).Result;
            if (user == null)
            {
                throw new Exception("User not found");

            }
            User owner = _userRepository.GetById(id).Result;
            if (owner == null)
            {
                throw new Exception("User not found");

            }
            Property property = null;
            Device device = null;
            SharedType type = SharedType.PROPERTY;
            if (dto.SharedType.ToLower() == "property")
            {
                property = _propertyRepository.GetById(dto.PropertyId);
                if (property == null)
                {
                    throw new Exception("Property not found");
                };
            } 
            else
            {
                device = _deviceRepository.GetById(dto.DeviceId).Result;
                if (device == null)
                {
                    throw new Exception("Property not found");
                };
                type = SharedType.DEVICE;
            }
            SharedStatus status = SharedStatus.PENDING;
            SharedDevices devices = new SharedDevices();
            devices.PropertyId = property?.Id;
            devices.Status = status;
            devices.DeviceId = device?.Id;
            devices.SharedType = type;
            devices.UserId = user.Id;
            devices.OwnerId = owner.Id;
            _deviceRepository.AddSharedDevice(devices);
        }

        public void UpdateSharedDevice(int id, SharedStatus status)
        {
            _deviceRepository.UpdateSharedDevices(id, status);
        }
    }
}
