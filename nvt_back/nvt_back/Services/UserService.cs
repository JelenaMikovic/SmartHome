using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.DTOs;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;

namespace nvt_back.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;

        }

        public void CreateUser(CreateUserDTO userDTO)
        {
            if (_userRepository.GetByEmail(userDTO.Email) != null) throw new Exception("User already exists");
            User user = new User();
            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.Password = userDTO.Password;
            user.IsActivated = false;
            user.Surname = userDTO.Surname;
            user.Role = Enum.Parse<UserRole>(userDTO.Role);
            _userRepository.AddUser(user);
        }

        public Task<User> GetByEmailAndPassword(string email, string password)
        {
            return _userRepository.GetByEmailAndPassword(email, password);
        }
    }
}
