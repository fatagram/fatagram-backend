using Fatagram.Application.Dtos;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.Interfaces
{
    public interface IPrivacySettingsService
    {
        /// <summary>
        /// Set a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <param name="privacyLevel"></param>
        /// <returns></returns>
        Task<Result<string>> SetPrivacySettingAsync(string userId, PrivacySettingDto privacySettingDto);


        /// <summary>
        /// Set a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="privacySettingDto"></param>
        /// <returns></returns>
        Task<Result<string>> SetPrivacySettingAsync(Guid userId, PrivacySettingDto privacySettingDto);


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<Result<string>> GetPrivacyLevelAsync(string userId, string field);


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<Result<string>> GetPrivacyLevelAsync(Guid userId, string field);

    }
}
