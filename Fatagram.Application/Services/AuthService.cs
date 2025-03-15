using Fatagram.Application.Services.Interfaces;
using Fatagram.Infrastructure.Repositories.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Application.Dtos;

namespace Fatagram.Application.Services
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
            var account = await _accountRepository.GetAccountByUsernameAsync(request.Username);

            if (account is null) return Result<string>.Failure(ErrorCodes.ACCOUNT_NOT_FOUND);
            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash)) 
                return Result<string>.Failure(ErrorCodes.WRONG_PASSWORD);

            return Result<string>.Success("LOGIN_SUCCESS");
        }
    }
}
