using Fatagram.Application.Dtos.User;
using Fatagram.Domain.Enums;
using Fatagram.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserPrivacyServices.Interface
{
    /// <summary>
    /// User privacy service
    /// </summary>
    public interface IUserPrivacyService
    {
        /// <summary>
        /// Update user privacy
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result<string>> UpdateUserPrivacyAsync(string userId, UpdateUserPrivacyDto request);
    }
}
