using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface
{
    /// <summary>
    /// Interface for user privacy repository to manage privacy levels of user fields.
    /// </summary>
    public interface IUserPrivacyRepository
    {
        /// <summary>
        /// Gets the privacy level of a specified field for a user identified by a GUID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="field">The field for which the privacy level is requested.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the privacy level of the specified field.</returns>
        Task<Dictionary<string, PrivacyLevel>> GetPrivacyLevelsAsync(Guid userId, IEnumerable<string> field);

        /// <summary>
        /// Gets the privacy level of a specified field for a user identified by a string ID.
        /// </summary>
        /// <param name="userId">The string identifier of the user.</param>
        /// <param name="field">The field for which the privacy level is requested.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the privacy level of the specified field.</returns>
        Task<Dictionary<string, PrivacyLevel>> GetPrivacyLevelsAsync(string userId, IEnumerable<string> field);

        /// <summary>
        /// Sets the privacy level of all fields for a user identified by a GUID to private.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A task that represents a asynchronous operation</returns>
        Task SetAllPublicAsync(string userId);

        /// <summary>
        /// Sets the privacy level of all fields for a user identified by a GUID to private.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A task that represents a asynchronous operation</returns>
        Task SetAllPublicAsync(Guid userId);

        /// <summary>
        /// Sets the privacy level of all fields for a user identified by a GUID to private.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userPrivacy"></param>
        /// <returns></returns>
        Task SetPrivacyLevelAsync(UserPrivacy userPrivacy);

    }
}
