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

            ActivationCode activationCode = new ActivationCode();
            activationCode.User = user;
            activationCode.Code = Guid.NewGuid().ToString();
            activationCode.Expiration = DateTime.UtcNow.AddDays(1);

            _userRepository.AddActivationCode(activationCode);
            _ = sendEmail(activationCode);
        }

        public async Task sendEmail(ActivationCode activationCode)
        {
            var apiKey = "SEND_GRID_API_KEY";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("?", "La Casa De Smart");
            var subject = "Account activation";
            var to = new EmailAddress(activationCode.User.Email, activationCode.User.Name);
            var plainTextContent = $"Hello {activationCode.User.Name}! Your activation code is: {activationCode.Code}";
            var htmlContent = $"<strong>{activationCode.Code}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
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
