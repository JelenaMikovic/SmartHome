using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private DatabaseContext _context;

        public AddressRepository(DatabaseContext context)
        {
            this._context = context;
        }

        public void Add(Address address)
        {
            this._context.Addresses.Add(address);
        }
    }
}
