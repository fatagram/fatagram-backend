

namespace Fatagram.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interface for the privacy setting repository
    /// </summary>
    public interface IPrivarySettingRepository
    {
        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <param name="privacyLevel"></param>
        /// <returns></returns>
        Task<bool> SetPrivacySettingAsync(Guid userId, string field, string privacyLevel);

        /// <summary>
        /// Set a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <param name="privacyLevel"></param>
        /// <returns></returns>
        Task<bool> SetPrivacySettingAsync(string userId, string field, string privacyLevel);



        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<string> GetPrivacyLevelAsync(Guid userId, string field);


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<string> GetPrivacyLevelAsync(string userId, string field);



        /// <summary>
        /// Get all privacy settings for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetAllPrivacySettingsAysnc(string userId);


        /// <summary>
        /// Set all privacy settings to public
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> InitialPrivacySettingForNewUserAsync (string userId);


        /// <summary>
        /// Set all privacy settings to public
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> InitialPrivacySettingForNewUserAsync(Guid userId);
    }
}
