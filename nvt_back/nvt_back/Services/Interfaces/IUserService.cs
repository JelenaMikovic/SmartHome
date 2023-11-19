using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface IUserService
    {
        void CreateUser(CreateUserDTO userDTO);
        Task<User> GetByEmailAndPassword(string email, string password);
    }
}
