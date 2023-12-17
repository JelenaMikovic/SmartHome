using Microsoft.EntityFrameworkCore;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class LampRepository : ILampRepository
    {
        private readonly DatabaseContext _context;

        public LampRepository(DatabaseContext context)
        {
            _context = context;
        }


        public async Task<Lamp> GetById(int id)
        {
            return await _context.Lamps.Where(device => device.Id == id).Include(device => device.Property).ThenInclude(property => property.Address).FirstOrDefaultAsync();
        }
    }
}
