using nvt_back.Model.Devices;
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

        public Device GetById(int deviceId)
        {
            return _context.Devices.FirstOrDefault(device => device.Id == deviceId);
        }

        public Device ChangeOnlineStatus(int deviceId, bool activate)
        {
            var device = GetById(deviceId);
            if (device == null)
                throw new KeyNotFoundException("Device with id: " + deviceId.ToString() + " doesn't exist!");
            if (device.IsOnline == activate)
                return device;
            device.IsOnline = activate;
            _context.SaveChanges();
            return device;
        }

        public List<Device> GetOnlineDevices()
        {
            return _context.Devices.Where(device => device.IsOnline).ToList();
        }
    }
}
