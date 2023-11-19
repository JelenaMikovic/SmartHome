namespace nvt_back.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAndPassword(string email, string password);
    }
}
