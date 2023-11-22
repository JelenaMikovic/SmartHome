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

        public void AddLamp(Lamp lamp)
        {
            _context.Lamps.Add(lamp);
            _context.SaveChanges();
        }
    }
}
