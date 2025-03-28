using Fatagram.Application.Utils;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Shared.Utils;

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
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        // Constructor
        public AuthService(IUserRepository userRepository,
            IAccountRepository accountRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="request">
        ///     Information about user's login
        /// </param>
        /// <returns></returns>
        public async Task<Result<string>> Login(LoginDto request)
        {
            try
            {
                var getAccountResult = await _accountRepository.GetAccountByUsernameAsync(request.Username);
                if (getAccountResult is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);

                if (!BCrypt.Net.BCrypt.Verify(request.Password, getAccountResult.PasswordHash))
                    return Result<string>.Failure(ErrorCodes.WRONG_PASSWORD);

                return Result<string>.Success("LOGIN_SUCCESS");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure("LOGIN_FAILED");
            }
        }
    }
}
