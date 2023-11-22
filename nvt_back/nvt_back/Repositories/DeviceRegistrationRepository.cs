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

        public void Add(Device device)
        {
            _context.Devices.Add(device);
            _context.SaveChanges();
        }
    }
}
