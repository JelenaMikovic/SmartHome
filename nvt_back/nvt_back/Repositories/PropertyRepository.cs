using Microsoft.EntityFrameworkCore;
using nvt_back.DTOs;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly DatabaseContext _context;

        public PropertyRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void Add(Property property)
        {
            _context.Properties.Add(property);
            _context.SaveChanges();
        }

        public IEnumerable<Property> getAll()
        {
            return _context.Properties.Include(c => c.Address).ToList();
        }

        public IEnumerable<Property> GetAllPaginated(int page, int size)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Property> GetAllPaginatedForOwner(int page, int size, int id)
        {
            IEnumerable<Property> properties = _context.Properties.Where(x => x.UserId == id).OrderByDescending(x => x.Id).Include(x => x.Address).Include(x => x.Address.City).Include(x => x.Address.City.Country)
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

            return properties;
        }

        public int GetCount()
        {
            return this._context.Properties.Count();
        }

        public int GetCountForOwner(int id)
        {
            return this._context.Properties.Where(x => x.UserId == id).Count();
        }

    }
}
