using AutoMapper;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Constants;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.AuthServices
{
    /// <summary>
    /// AuthService class intehirating IAuthService
    /// </summary>
    public class AuthService : IAuthService
    {
        // AppDbContext Instance
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailRepository _emailRepository;

        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService()
        {
            _userRepository = null!;
            _accountRepository = null!;
            _emailRepository = null!;
            _tokenService = null!;
            _mapper = null!;
        }

        // Constructor
        public AuthService(
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IEmailRepository emailRepository,
            ITokenService tokenService,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _emailRepository = emailRepository;

            _tokenService = tokenService;
            _mapper = mapper;
        }

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="request">
        ///     Information about user's login
        /// </param>
        /// <returns></returns>
        public async Task<Result<TokenResponseDto>> Login(LoginDto request)
        {
            var account =
                (
                    await _accountRepository.GetAllAsync<Account>(a =>
                        a.Username == request.Username
                    )
                ).FirstOrDefault() ?? throw new AppException(Errors.Auth.AccountNotFound);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
                throw new AppException(Errors.Auth.InvalidCredentials);

            var user =
                await _userRepository.GetByUsernameAsync(request.Username)
                ?? throw new AppException(Errors.Auth.UserNotFound);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(account.Id);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(account.Id);

            return Result<TokenResponseDto>.Create(
                ResponseStatusCode.Success,
                new() { AccessToken = accessToken, RefreshToken = refreshToken }
            );
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result<TokenResponseDto>> Register(RegisterDto registerDto)
        {
            var account = await _accountRepository.GetAllAsync(
                a => a.Username == registerDto.Username,
                s => s.Username
            );
            if (account?.Count > 0)
            {
                throw new AppException(Errors.Auth.UsernameExisted);
            }
            var email = await _emailRepository.GetAllAsync(
                e => e.Address == registerDto.Email,
                s => s.Address
            );
            if (email?.Count > 0)
            {
                throw new AppException(Errors.Auth.EmailExisted);
            }
            // Get email, phone, name from registerDto to newUser
            var newUser = _mapper.Map<User>(registerDto);

            // Get username and password from registerDto to newAccount
            var newAccount = _mapper.Map<Account>(registerDto);
            newAccount.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            newUser.Accounts.Add(newAccount);
            await _userRepository.AddAsync(newUser);

            return Result<TokenResponseDto>.Create(ResponseStatusCode.Created);
        }

        /// <summary>
        /// Change password method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        public async Task<Result<string>> ChangePasswordAsync(
            Guid userId,
            ChangePasswordDto changePasswordRequest
        )
        {
            var res =
                await _accountRepository.GetByEmailAsync(userId.ToString())
                ?? throw new AppException(Errors.Auth.AccountNotFound);
            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, res.PasswordHash))
            {
                throw new AppException(Errors.Auth.Unauthorized);
            }
            res.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
            await _accountRepository.UpdateAsync(res);

            return Result<string>.Create();
        }
    }
}
