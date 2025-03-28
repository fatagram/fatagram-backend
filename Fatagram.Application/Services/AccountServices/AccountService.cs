using Fatagram.Domain.Models;
using Fatagram.Application.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AutoMapper;
using Fatagram.Application.Services.AccountServices.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Account;
using Fatagram.Shared.Utils;
using Fatagram.Infrastructure.Exceptions.AccountException;
using Fatagram.Infrastructure.Exceptions.UserExceptions;
using System.Runtime.Serialization;

namespace Fatagram.Application.Services.AccountServices
{
    /// <summary>
    /// Account service
    /// </summary>
    public class AccountService : IAccountService
    {
        // Dependency injection
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;



        // Constructor
        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<string>> Register(RegisterDto registerDto)
        {
            try
            {
                // Get email, phone, name from registerDto to newUser
                var newUser = _mapper.Map<User>(registerDto);

                // Get username and password from registerDto to newAccount
                var newAccount = _mapper.Map<Account>(registerDto);
                newAccount.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                
                newUser.FullName = $"{registerDto.FirstName} {registerDto.LastName}";
                newUser.Accounts.Add(newAccount);
                await _userRepository.CreateUserAsync(newUser);
                
                return Result<string>.Success("REGISTER_SUCCESS");
            }
            catch (Exception)
            {
                return Result<string>.Failure("REGISTER_FAILED");
            }
        }


        /// <summary>
        /// Change password method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordRequest)
        {
            try
            {
                var res = await _accountRepository.GetAccountByUserIdAsync(userId);
                if (res is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND, $"Cannot find account {userId}");

                if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, res.PasswordHash))
                {
                    return Result<string>.Failure(ErrorCodes.WRONG_PASSWORD, "Invalid password");
                }

                res.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
                await _accountRepository.UpdateAccountAsync(res);

                return Result<string>.Success("CHANGE_PASSWORD_SUCCESS");
            }
            catch (Exception)
            {
                return Result<string>.Failure("CHANGE_PASSWORD_FAILED");
            }
        }
    }
}
