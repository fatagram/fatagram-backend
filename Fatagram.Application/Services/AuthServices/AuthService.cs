using System.Runtime.CompilerServices;
using AutoMapper;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Abstractions.Security;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.AuthServices;
using Fatagram.Application.Services.AuthServices.OAuth;
using Fatagram.Application.Services.TokenServices;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
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
        IUserEmailRepository userEmailRepository,
        ITokenService tokenService,
        OAuthServiceFactory oauthServiceFactory,
        IPasswordHasher passwordHasher,
        IMapper mapper,
        ILogger<AuthService> logger
    ) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IUserEmailRepository _userEmailRepository = userEmailRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly OAuthServiceFactory _oauthServiceFactory = oauthServiceFactory;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="request">
        ///     Information about user's login
        /// </param>
        /// <returns></returns>
        public async Task<Result<TokenDto>> Login(LoginDto request)
        {
            var account = await ResolveLoginAccountAsync(request.UsernameOrEmail);
            VerifyPassword(account, request.Password);

            var accessToken = _tokenService.GenerateAccessToken(account.UserId);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(account.UserId);

            return Result<TokenDto>.Create(
                ResponseStatusCode.Success,
                new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken }
            );
        }

        private async Task<Account> ResolveLoginAccountAsync(string usernameOrEmail)
        {
            var account = await _accountRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
            if (account == null)
                throw new AppException(Errors.Auth.InvalidCredentials);
            return account;
        }

        private void VerifyPassword(Account account, string password)
        {
            if (account.PasswordHash == null)
                throw new AppException(Errors.Auth.InvalidCredentials);

            if (!_passwordHasher.Verify(password, account.PasswordHash))
                throw new AppException(Errors.Auth.InvalidCredentials);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Result> Register(RegisterDto registerDto)
        {
            var account = await _accountRepository.GetAllAsync(
                a => a.Username == registerDto.Username,
                s => s.Username
            );
            if (account?.Count > 0)
                throw new AppException(Errors.Auth.UsernameExisted);
            var emailExists = await _userEmailRepository.IsEmailInUseAsync(registerDto.Email);
            if (emailExists)
                throw new AppException(Errors.Auth.EmailExisted);

            // Get email, phone, name from registerDto to newUser
            var newUser = _mapper.Map<User>(registerDto);
            var newAccount = _mapper.Map<Account>(registerDto);

            newAccount.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
            newUser.Accounts.Add(newAccount);

            var user = await _userRepository.AddAsync(newUser);

            return Result<TokenDto>.Create(ResponseStatusCode.Success);
        }

        /// <summary>
        /// Change password method
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordRequest"></param>
        /// <returns></returns>
        public Task<Result<string>> ChangePasswordAsync(
            Guid userId,
            ChangePasswordDto changePasswordRequest
        )
        {
            // var res =
            //     await _accountRepository.GetByEmailAsync(userId.ToString())
            //     ?? throw new AppException(Errors.Auth.AccountNotFound);
            // if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, res.PasswordHash))
            // {
            //     throw new AppException(Errors.Auth.Unauthorized);
            // }
            // res.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);
            // await _accountRepository.UpdateAsync(res);

            // return Result<string>.Create(
            //     ResponseStatusCode.Success,
            //     "Change password successfully"
            // );
            throw new NotImplementedException();
        }

        /// <summary>
        /// OAuth callback - authenticate user using OAuth provider
        /// </summary>
        /// <param name="provider">OAuth provider type</param>
        /// <param name="code">OAuth authorization code</param>
        /// <returns>Token response</returns>
        public async Task<Result<TokenDto>> OAuthCallback(
            OAuthProvider provider,
            string code,
            string? redirectUri = null
        )
        {
            _logger.LogInformation(
                "{Provider} OAuth callback received with code: {Code}",
                provider,
                code
            );
            var oauthService = _oauthServiceFactory.CreateService(provider);

            var userInfo = await oauthService.GetUserInfoAsync(code, redirectUri);
            _logger.LogInformation(
                "{Provider} OAuth user info retrieved: {Email}, {Name}",
                provider,
                userInfo.Email,
                userInfo.Name
            );

            if (string.IsNullOrEmpty(userInfo.Email))
            {
                throw new AppException(
                    new Error("OAUTH_ERROR", "Email not found from OAuth provider")
                );
            }

            var account = await _accountRepository.GetByUsernameOrEmailAsync(userInfo.Email);
            _logger.LogInformation("Searching for account with email: {Email}", userInfo.Email);

            string accessToken;
            string refreshToken;

            if (account is null)
            {
                var newAccount = await CreateNewUserFromOAuthAsync(userInfo);

                _logger.LogInformation(
                    "New account created with ID: {AccountId}, UserId: {UserId}",
                    newAccount.Id,
                    newAccount.UserId
                );

                accessToken = _tokenService.GenerateAccessToken(newAccount.UserId);
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(newAccount.UserId);
            }
            else
            {
                _logger.LogInformation(
                    "Logging in existing user with email: {Email}",
                    userInfo.Email
                );
                _logger.LogInformation(
                    "Account ID: {AccountId}, User ID: {UserId}",
                    account.Id,
                    account.UserId
                );

                accessToken = _tokenService.GenerateAccessToken(account.UserId);
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(account.UserId);
            }

            return Result<TokenDto>.Create(
                ResponseStatusCode.Success,
                new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken }
            );
        }

        /// <summary>
        /// Create new user from OAuth user info
        /// </summary>
        private async Task<Account> CreateNewUserFromOAuthAsync(OAuthUserInfo userInfo)
        {
            var newUser = new User
            {
                FullName = userInfo.Name,
                FirstName = userInfo.GivenName,
                LastName = userInfo.FamilyName,
                Avatar = userInfo.Picture,
                IsOnBoarding = false,
                UserEmails = new List<UserEmail>
                {
                    new UserEmail
                    {
                        Email = new Email { Address = userInfo.Email },
                        IsVerified = true,
                        IsPrimary = true,
                    },
                },
            };

            var newAccount = new Account { User = newUser };

            return await _accountRepository.AddAsync(newAccount);
        }

        /// <summary>
        /// Google OAuth callback (deprecated - use OAuthCallback instead)
        /// </summary>
        [Obsolete("Use OAuthCallback(OAuthProvider.Google, code) instead")]
        public async Task<Result<TokenDto>> GoogleCallback(OAuthCallbackDto request)
        {
            return await OAuthCallback(OAuthProvider.Google, request.Code);
        }
    }
}
