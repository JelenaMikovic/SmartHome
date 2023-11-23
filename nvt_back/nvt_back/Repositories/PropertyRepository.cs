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
    }
}
