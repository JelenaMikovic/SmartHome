using Microsoft.EntityFrameworkCore;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class CityRepository : ICityRepository
    {
        private DatabaseContext _context;

        public CityRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public IEnumerable<City> GetAll()
        {
            return _context.Cities.Include(c => c.Country).ToList();
        }
    }
}
