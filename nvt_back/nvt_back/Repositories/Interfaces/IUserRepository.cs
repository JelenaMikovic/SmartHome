using nvt_back.Model;

namespace nvt_back.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void AddActivationCode(ActivationCode activationCode);
        void AddUser(User user);
        Task<User> GetByEmail(string email);
        Task<ActivationCode> GetByUser(User user);
        Task<User> GetByEmailAndPassword(string email, string password);
        void ActivateUser(int userId);
        void ChangePassword(string newPassword, User user);
        Task<User> GetById(int userId);
    }
}
