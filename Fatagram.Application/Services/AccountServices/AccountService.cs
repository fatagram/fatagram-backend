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
using System.Runtime.Serialization;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

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
            var account = await _accountRepository.GetByUsernameAsync(registerDto.Username);
            if (account is not null)
            {
                return Result<string>.BadRequest(ErrorCodes.REGISTER_USERNAME_EXISTED);
            }
            var user = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (user is not null)
            {
                return Result<string>.BadRequest(ErrorCodes.EMAIL_EXISTED);
            }
            // Get email, phone, name from registerDto to newUser
            var newUser = _mapper.Map<User>(registerDto);

            // Get username and password from registerDto to newAccount
            var newAccount = _mapper.Map<Account>(registerDto);
            newAccount.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            newUser.FullName = $"{registerDto.FirstName} {registerDto.LastName}";
            newUser.Accounts.Add(newAccount);
            await _userRepository.AddAsync(newUser);

            return Result<string>.Success();
        }

        /// <summary>
        /// Change password method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordRequest)
        {
            var res = await _accountRepository.GetByUserIdAsync(userId);
            if (res is null)
            {
                throw new AccountNotFoundException();
            }
            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, res.PasswordHash))
            {
                throw new UnauthorizedException();
            }
            res.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
            await _accountRepository.UpdateAsync(res);

            return Result<string>.Success();
        }   

        // For admin 
        public async Task<Result<AccountsDto>> GetAccountsAsync(int page, int pageSize, string? username = null)
        {
            var result = await _accountRepository.GetAccountsAsync(page, pageSize, username);
            var accountsDto = _mapper.Map<IEnumerable<AccountDto>>(result.accounts);
            return Result<AccountsDto>.Success(new AccountsDto
            {
                Accounts = accountsDto.ToList(),
                TotalPage = result.totalPage,
                TotalAccount = result.totalAccount
            });
        }
    }
}
