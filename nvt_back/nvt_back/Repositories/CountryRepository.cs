using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private DatabaseContext _context;

        public CountryRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public IEnumerable<Country> GetAll()
        {
            return this._context.Countries.ToList();
        }
    }
}
