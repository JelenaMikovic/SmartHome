using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class DeviceRegistrationRepository : IDeviceRegistrationRepository
    {
        private readonly DatabaseContext _context;

        public DeviceRegistrationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Add(Device device)
        {
            await _context.Devices.AddAsync(device);
            await _context.SaveChangesAsync();
        }
    }
}
