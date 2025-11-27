using System.Runtime.CompilerServices;
using AutoMapper;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Application.Services.TokenServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data.Extensions;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.EmailRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Common;
using Fatagram.Shared.Constants;
using Fatagram.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.AuthServices
{
    /// <summary>
    /// AuthService class intehirating IAuthService
    /// </summary>
    public class AuthService(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IEmailRepository emailRepository,
        ITokenService tokenService,
        GoogleOAuthService googleOAuthService,
        IMapper mapper,
        ILogger<AuthService> logger
    ) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IEmailRepository _emailRepository = emailRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly GoogleOAuthService _googleOAuthService = googleOAuthService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="request">
        ///     Information about user's login
        /// </param>
        /// <returns></returns>
        public async Task<Result<TokenResponseDto>> Login(LoginDto request)
        {
            var accountByEmail = await _accountRepository.GetByEmailAsync(request.UsernameOrEmail);
            var accountByUsername = await _accountRepository.GetByUniqueKeyAsync<Account, string>(
                u => u.Username,
                request.UsernameOrEmail
            );

            if (accountByEmail == null && accountByUsername == null)
            {
                throw new AccountNotFoundException();
            }

            var passwordHash = accountByEmail?.PasswordHash ?? accountByUsername?.PasswordHash;
            var accountId = accountByEmail?.Id ?? accountByUsername?.Id;
            if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
            {
                throw new BadRequestException(Errors.Auth.PasswordIncorrect);
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(accountId ?? Guid.Empty);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(
                accountId ?? Guid.Empty
            );
            return Result<TokenResponseDto>.Create(
                ResponseStatusCode.Success,
                new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken }
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
            newAccount.Emails = [new() { Address = registerDto.Email, IsPrimary = true }];
            newUser.Accounts.Add(newAccount);
            var user = await _userRepository.AddAsync(newUser);

            var accessToken = await _tokenService.GenerateAccessTokenAsync(
                user.Accounts.FirstOrDefault()?.Id ?? Guid.Empty
            );
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(
                user.Accounts.FirstOrDefault()?.Id ?? Guid.Empty
            );

            return Result<TokenResponseDto>.Create(
                ResponseStatusCode.Success,
                new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken }
            );
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

            return Result<string>.Create(
                ResponseStatusCode.Success,
                "Change password successfully"
            );
        }

        public async Task<Result<TokenResponseDto>> GoogleCallback(GoogleCallbackDto request)
        {
            // Step 1: Exchange authorization code for access token
            var oauthResponse = await _googleOAuthService.ExchangeCodeAsync(request.Code);
            _logger.LogInformation(
                "Google OAuth token received, expires in: {ExpiresIn}s",
                oauthResponse.ExpiresIn
            );

            // Step 2: Get user info from Google using access token
            var userInfo = await _googleOAuthService.GetUserInfoAsync(oauthResponse.AccessToken);
            _logger.LogInformation(
                "Google user info: {Email}, {Name}, {Picture}",
                userInfo.Email,
                userInfo.Name,
                userInfo.Picture
            );

            if (string.IsNullOrEmpty(userInfo.Email))
            {
                throw new AppException(new Error("LOGIN_WITH_GOOGLE_ERROR", "Email not found"));
            }

            // Step 3: Check if account exists
            var account = await _accountRepository.GetByEmailAsync(userInfo.Email);
            string accessToken;
            string refreshToken;

            if (account is null)
            {
                // Step 4a: Create new user if not exists
                _logger.LogInformation("Creating new user for email: {Email}", userInfo.Email);

                var newUser = new User
                {
                    FullName = userInfo.Name,
                    FirstName = userInfo.GivenName,
                    LastName = userInfo.FamilyName,
                    Avatar = userInfo.Picture,
                    IsOnBoarding = false,
                    Accounts =
                    [
                        new Account
                        {
                            Emails = [new Email { Address = userInfo.Email, IsPrimary = true }],
                        },
                    ],
                };

                var user =
                    await _userRepository.AddAsync(newUser)
                    ?? throw new AppException(
                        new Error("LOGIN_WITH_GOOGLE_ERROR", "Failed to create user")
                    );

                _logger.LogInformation("New user created with ID: {UserId}", user.Id);

                accessToken = await _tokenService.GenerateAccessTokenAsync(
                    user.Accounts.First().Id
                );
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(
                    user.Accounts.First().Id
                );

                _logger.LogInformation("New user created with ID: {UserId}", user.Id);
            }
            else
            {
                // Step 4b: Login existing user
                _logger.LogInformation("Logging in existing user: {Email}", userInfo.Email);
                _logger.LogInformation("Existing user ID: {UserId}", account.Id);

                accessToken = await _tokenService.GenerateAccessTokenAsync(account.Id);
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(account.Id);
            }

            return Result<TokenResponseDto>.Create(
                ResponseStatusCode.Success,
                new TokenResponseDto { AccessToken = accessToken, RefreshToken = refreshToken }
            );
        }
    }
}
