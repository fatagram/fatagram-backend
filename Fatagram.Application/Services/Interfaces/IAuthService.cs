using Fatagram.Domain.Enums;
using Fatagram.Application.Dtos;
using Fatagram.Application.Utils;
using System.Security.Claims;

namespace Fatagram.Application.Services.Interfaces
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
