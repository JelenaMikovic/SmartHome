using Microsoft.EntityFrameworkCore;
using nvt_back.Model;
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

        public void AddActivationCode(ActivationCode activationCode)
        {
            _context.ActivationCodes.Add(activationCode);
        }

        public Task<ActivationCode> GetByUser(User user)
        {
            return _context.ActivationCodes.FirstOrDefaultAsync(u => u.User == user);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public Task<User> GetByEmail(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User> GetByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public void ActivateUser(int userId)
        {
            User user = _context.Users.FirstOrDefaultAsync(u => u.Id == userId).Result;
            user.IsActivated = true;
            _context.SaveChanges();
        }
    }
}
