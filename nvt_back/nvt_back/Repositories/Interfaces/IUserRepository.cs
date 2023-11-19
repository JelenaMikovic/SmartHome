namespace nvt_back.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        Task<User> GetByEmail(string email);
        Task<User> GetByEmailAndPassword(string email, string password);
    }
}
