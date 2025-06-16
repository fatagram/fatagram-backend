using Fatagram.Domain.Enums;
using System.Security.Claims;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Utils;
using Fatagram.Application.Dtos.Token;

namespace Fatagram.Application.Services.AuthServices.Interface
{
    /// <summary>
    /// Interface for the authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<LoginResponseDto>> Login(LoginDto loginDto);

    }
}
