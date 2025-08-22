using Fatagram.Application.Utils;
using Fatagram.Application.Services.AuthServices.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.RefreshTokenRepository.Interface;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Token;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

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
        public async Task<Result<LoginResponseDto>> Login(LoginDto request)
        {
            var getAccountResult = await _accountRepository.GetByUsernameAsync(request.Username);
            if (getAccountResult is null) throw new AccountNotFoundException();

            if (!BCrypt.Net.BCrypt.Verify(request.Password, getAccountResult.PasswordHash))
                throw new UnauthorizedException(ErrorCodes.WRONG_PASSWORD, "Password is not correct.");

            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user is null)
                throw new UserNotFoundException();

            return Result<LoginResponseDto>.Success(new LoginResponseDto()
            {
                UserId = user.Id.ToString(),
                UrlName = user.UrlName,
            });
        }
    }
}
