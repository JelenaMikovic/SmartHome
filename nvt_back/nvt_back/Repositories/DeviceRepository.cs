using Microsoft.EntityFrameworkCore;
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
            return await _context.Devices.FirstOrDefaultAsync(device => device.Id == deviceId);
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
    }
}
