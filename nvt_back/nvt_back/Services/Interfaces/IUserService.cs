﻿using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface IUserService
    {
        void ActivateAccount(ActivationDTO activationDTO);
        void ChangePassword(ChangePasswordDTO changePasswordDTO, string email);
        void CreateUser(CreateUserDTO userDTO);
        Task<User> GetByEmailAndPassword(string email, string password);
    }
}
