using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.DTOs;
using System.Configuration;
using System.Security.Claims;
using System.Security.Principal;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using nvt_back.Model;

namespace nvt_back.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageService _imageService;
        private readonly IMailService _mailService;

        public UserService(IUserRepository userRepository, IImageService imageService)
        {
            this._userRepository = userRepository;
            _imageService = imageService;   
        }

        public void CreateUser(CreateUserDTO userDTO)
        {
            if (_userRepository.GetByEmail(userDTO.Email).Result != null) throw new Exception("User already exists");
            User user = new User();
            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.Password = userDTO.Password;
            user.IsActivated = false;
            user.Surname = userDTO.Surname;
            user.Role = Enum.Parse<UserRole>(userDTO.Role);
            //string filePath = _imageService.SaveImage(userDTO.Image);
            //user.ImagePath = filePath;

            _userRepository.AddUser(user);

            ActivationCode activationCode = new ActivationCode();
            activationCode.User = user;
            activationCode.Code = Guid.NewGuid().ToString();
            activationCode.Expiration = DateTime.UtcNow.AddDays(1);

            _userRepository.AddActivationCode(activationCode);
            _mailService.SendAccountActiationEmail(user.Email, user.Name, activationCode.Code);
        }

        public Task<User> GetByEmailAndPassword(string email, string password)
        {
            return _userRepository.GetByEmailAndPassword(email, password);
        }

        public void ActivateAccount(ActivationDTO activationDTO)
        {
            User user = _userRepository.GetByEmail(activationDTO.Email).Result;
            if (user == null) {
                throw new Exception("Email not found.");
            }
            ActivationCode activationCode= _userRepository.GetByUser(user).Result;
            if(activationCode.Expiration < DateTime.Now)
            {
                throw new Exception("Code expired.");
            }
            if(activationCode.Code != activationDTO.Code)
            {
                throw new Exception("Code invalid.");
            }
            _userRepository.ActivateUser(user.Id);
        }

    }
}
