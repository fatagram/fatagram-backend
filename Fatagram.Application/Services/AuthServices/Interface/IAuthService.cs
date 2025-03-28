using Fatagram.Domain.Enums;
using System.Security.Claims;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Shared.Utils;

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
        Task<Result<string>> Login(LoginDto loginDto);

    }
}
