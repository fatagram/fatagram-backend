using AutoMapper;
using Fatagram.Application.Dtos.Account;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace Fatagram.Application.Services.AuthService
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
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        // Constructor
        public AuthService(
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IEmailRepository emailRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _emailRepository = emailRepository;
            _refreshTokenRepository = refreshTokenRepository;
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
            var account = (
                await _accountRepository.GetAllAsync<Account>(a => a.Username == request.Username)
            ).FirstOrDefault();
            if (account is null)
                throw new AccountNotFoundException();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
                throw new BadRequestException(
                    "INVALID_CREDENTIALS",
                    "The provided credentials are invalid"
                );

            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user is null)
                throw new UserNotFoundException();

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
            if (account.Count() > 0)
            {
                throw new AppException("USERNAME_EXISTED", "Username already exists.");
            }
            var email = await _emailRepository.GetAllAsync(
                e => e.Address == registerDto.Email,
                s => s.Address
            );
            if (email.Count() > 0)
            {
                throw new AppException("EMAIL_EXISTED", "Email already exists.");
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
            var res = await _accountRepository.GetByEmailAsync(userId.ToString());
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

            return Result<string>.Create();
        }
    }
}
