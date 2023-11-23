using Microsoft.EntityFrameworkCore;
using nvt_back.Repositories.Interfaces;

namespace nvt_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public Task<User> GetByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
