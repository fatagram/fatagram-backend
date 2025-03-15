using Fatagram.Domain.Models;
using Fatagram.Application.Dtos;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Fatagram.Application.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Fatagram.Application.Services.Interfaces;
using AutoMapper;

namespace Fatagram.Application.Services
{
    /// <summary>
    /// Account service
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPrivarySettingRepository _privacySettingRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IPrivarySettingRepository privacySettingRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _privacySettingRepository = privacySettingRepository;
            _mapper = mapper;
        }



        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<string>> Register(RegisterDto registerDto)
        {
            var account = await _accountRepository.GetAccountByUsernameAsync(registerDto.Username);
            if (account is null)
            {
                var userId = Guid.NewGuid();
                var newUser = new User();
                newUser.Id = userId;
                _mapper.Map(registerDto, newUser);

                var createUserSuccess = await _userRepository.CreateUserAsync(newUser);
                if (!createUserSuccess) return Result<string>.Failure(ErrorCodes.REGISTER_FAILED);

                var createAccountSuccess = await _accountRepository.CreateAccountAsync(new Account()
                {
                    Id = Guid.NewGuid(),
                    Username = registerDto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    CreatedAt = DateTime.Now,
                    UserId = userId,
                    User = newUser
                });
                if (!createAccountSuccess) return Result<string>.Failure(ErrorCodes.REGISTER_FAILED);
                
                await _privacySettingRepository.InitialPrivacySettingForNewUserAsync(userId);
                Debug.WriteLine("NEW ACCOUNT ID: " + userId.ToString());

                return Result<string>.Success("REGISTER_SUCCESS");
            }
            return Result<string>.Failure(ErrorCodes.REGISTER_USERNAME_EXISTED);
        }


        /// <summary>
        /// Change password method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordRequest)
        {
            var account = await _accountRepository.GetAccountByUserIdAsync(userId);

            if (account is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);
            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, account.PasswordHash))
            {
                return Result<string>.Failure(ErrorCodes.WRONG_PASSWORD);
            }
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
            var result = await _accountRepository.UpdateAccountAsync(account);
            return result ? Result<string>.Success("CHANGE_PASSWORD_SUCCESS") : Result<string>.Failure("CHANGE_PASSWORD_FAILED");
        }
    }
}
