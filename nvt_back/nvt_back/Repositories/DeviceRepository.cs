using Microsoft.EntityFrameworkCore;
using nvt_back.DTOs.DeviceDetailsDTO;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DatabaseContext _context;

        public DeviceRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Device> GetById(int deviceId)
        {
            return await _context.Devices.Include(device => device.Property).ThenInclude(property => property.Address).FirstOrDefaultAsync(device => device.Id == deviceId);
        }

        public async Task<List<Device>> GetAll()
        {
            return await _context.Devices.ToListAsync();
        }

        public async Task<List<Device>> GetOnlineDevices()
        {
            return await _context.Devices.Where(device => device.IsOnline).ToListAsync();
        }

        public async Task UpdateOnlineStatus(int deviceId, bool activate)
        {
            var device = await GetById(deviceId);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + deviceId.ToString() + " doesn't exist!");
            device.IsOnline = activate;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLatestHeartbeat(Heartbeat heartbeat, DateTime time)
        {
            var device = await GetById(heartbeat.DeviceId);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + heartbeat.DeviceId.ToString() + " doesn't exist!");
            device.LastHeartbeatTime = time;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ToggleState(int id, string status)
        {
            var device = await GetById(id);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + id.ToString() + " doesn't exist!");
            bool isTurnedOn = (status == "ON");


            if (device.DeviceType == DeviceType.SOLAR_PANEL)
            {
                SolarPanel solarPanel = (SolarPanel)device;
                if (solarPanel.IsOn == isTurnedOn)
                    return false;
                else
                {
                    solarPanel.IsOn = isTurnedOn;
                    await _context.SaveChangesAsync();
                    return true;
                }
            } 
            else
            {
                Lamp lamp = (Lamp)device;
                if (lamp.IsOn == isTurnedOn)
                    return false;
                else
                {
                    lamp.IsOn = isTurnedOn;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<int> GetDeviceCountForProperty(int propertyId)
        {
            return await _context.Devices.Where(x => x.PropertyId == propertyId).CountAsync();
        }

        public async Task<IEnumerable<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId, int page, int size)
        {
            List<Device> devices = await _context.Devices.Where(x => x.PropertyId == propertyId).OrderByDescending(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
            
            IEnumerable<DeviceDetailsDTO> details =  devices.Select(device => new DeviceDetailsDTO
            {
                Id = device.Id,
                Name = device.Name,
                PowerConsumption = device.PowerConsumption,
                PowerSource = device.PowerSource,
                Image = device.Image,
                IsOnline = device.IsOnline,
            });

            Console.WriteLine(details);
            return details;
        }

        public async Task<object> GetDetailsById(int id)
        {
            var device = await GetById(id);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + id.ToString() + " doesn't exist!");
            if (device.DeviceType == DeviceType.SOLAR_PANEL) {
                return await getSolarPanelDetailsById(device);
            } 
            else if (device.DeviceType == DeviceType.AMBIENT_SENSOR)
            {
                return await getAmbientSensorDetailsById(device);
            }
            else if (device.DeviceType == DeviceType.LAMP)
            {
                return await getLampDetailsById(device);
            }
            else if (device.DeviceType == DeviceType.HOME_BATTERY)
            {
                return await getBatteryDetailsById(device);
            }
            return null;
        }

        private async Task<object> getBatteryDetailsById(Device device)
        {
            HomeBattery battery = (HomeBattery)device;
            return new BatteryDetailsDTO
            {
                Id = battery.Id,
                IsOnline = battery.IsOnline,
                Name = battery.Name,
                PowerConsumption = battery.PowerConsumption,
                PowerSource = battery.PowerSource,
                Image = battery.Image,
                CurrentCharge = battery.CurrentCharge,
                Capacity = battery.Capacity,
                DeviceType = battery.DeviceType.ToString(),
            };
        }

        private async Task<object> getLampDetailsById(Device device)
        {
            Lamp lamp = (Lamp)device;
            return new LampDetailsDTO
            {
                Id = lamp.Id,
                IsOn = lamp.IsOn,
                IsOnline = lamp.IsOnline,
                Name = lamp.Name,
                PowerConsumption = lamp.PowerConsumption,
                PowerSource = lamp.PowerSource,
                Image = lamp.Image,
                DeviceType = lamp.DeviceType.ToString(),
                BrightnessLevel = lamp.BrightnessLevel,
                Regime = lamp.Regime.ToString()
            };
        }

        private async Task<object> getSolarPanelDetailsById(Device device)
        {
            SolarPanel sp = (SolarPanel)device;
            return new SolarPanelDetailsDTO
            {
                Id = sp.Id,
                Efficiency = sp.Efficiency,
                IsOn = sp.IsOn,
                IsOnline = sp.IsOn,
                Name = sp.Name,
                NumberOfPanels = sp.NumberOfPanels,
                PowerConsumption = sp.PowerConsumption,
                PowerSource = sp.PowerSource,
                Size = sp.Size,
                Image = sp.Image,
                DeviceType = sp.DeviceType.ToString()
            };
        }

        private async Task<object> getAmbientSensorDetailsById(Device device)
        {
            AmbientSensor amb = (AmbientSensor)device;
            return new AmbientSensorDetailsDTO
            {
                Id = amb.Id,
                IsOnline = amb.IsOnline,
                Name = amb.Name,
                PowerConsumption = amb.PowerConsumption,
                PowerSource = amb.PowerSource,
                Image = amb.Image,
                DeviceType = amb.DeviceType.ToString(),
                UpdateIntervalSeconds = amb.UpdateIntervalSeconds,
                CurrentHumidity = amb.CurrentHumidity,
                CurrentTemperature = amb.CurrentTemperature
            };
        }

        public async Task<List<int>> GetPropertyIdsWithHomeBatteries()
        {
            return await _context.Devices
                .Where(device => device.DeviceType == DeviceType.HOME_BATTERY)
                .Select(device => device.PropertyId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<HomeBattery>> GetAllBatteriesForPropertyId(int propertyId)
        {
            return await _context.Devices
                .Where(device => device.DeviceType == DeviceType.HOME_BATTERY && device.PropertyId == propertyId)
                .Select(device => (HomeBattery)device)
                .ToListAsync();
        }

        public async Task<List<Device>> GetConsumingPowerDevicesForProperty(int propertyId)
        {
            var devices = await _context.Devices
            .Where(device => device.PowerSource != 0 && device.PropertyId == propertyId).ToListAsync();
            List<Device> workingDevices = new List<Device>();
            foreach (var device in devices)
            {
                if (device.DeviceType == DeviceType.SOLAR_PANEL)
                {
                    var sp = (SolarPanel)device;
                    if (sp.IsOn)
                        workingDevices.Add(sp);
                }
                else
                    workingDevices.Add(device);
            }
            return workingDevices;
        }
    }
}
