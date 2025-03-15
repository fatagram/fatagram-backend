using Fatagram.Application.Dtos;
using Fatagram.Application.Services.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Utils;
using Fatagram.Infrastructure.Repositories.Interfaces;

namespace Fatagram.Application.Services
{
    /// <summary>
    /// Service for the privacy settings
    /// </summary>
    public class PrivacySettingsService : IPrivacySettingsService
    {
        private readonly IPrivarySettingRepository _privacySettingRepository;

        public PrivacySettingsService(IPrivarySettingRepository privacySettingRepository)
        {
            _privacySettingRepository = privacySettingRepository;
        }


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<Result<string>> GetPrivacyLevelAsync(string userId, string field)
        {
            var res = await _privacySettingRepository.GetPrivacyLevelAsync(userId, field);
            return Result<string>.Success(res);
        }


        /// <summary>
        /// Get a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<Result<string>> GetPrivacyLevelAsync(Guid userId, string field) 
            => await GetPrivacyLevelAsync(userId.ToString(), field);


        /// <summary>
        /// Set a user's privacy settings
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="field"></param>
        /// <param name="privacyLevel"></param>
        /// <returns></returns>
        public async Task<Result<string>> SetPrivacySettingAsync(string userId, string field, string privacyLevel)
        {
            switch(field)
            {
                case Fields.FirstName: case Fields.LastName: case Fields.Email: case Fields.Phone:
                case Fields.Bio:
                case Fields.Avatar:
                     break;
                default:
                    return Result<string>.Failure("INVALID_FIELD");
            }

            switch (privacyLevel)
            {
                case "public": case "private": case "friends":
                    break;
                default:
                    return Result<string>.Failure("INVALID_PRIVACY_LEVEL");
            }

            var res = await _privacySettingRepository.SetPrivacySettingAsync(userId, field, privacyLevel);
            if (!res) return Result<string>.Failure("SET_PRIVACY_SETTING_FAILED");
            return Result<string>.Success("SET_PRIVACY_SETTING_SUCCESS");
        }

        public async Task<Result<string>> SetPrivacySettingAsync(string userId, PrivacySettingDto privacySettingDto) 
            => await SetPrivacySettingAsync(userId, privacySettingDto.Field, privacySettingDto.PrivacyLevel);

        public async Task<Result<string>> SetPrivacySettingAsync(Guid userId, PrivacySettingDto privacySettingDto)
            => await SetPrivacySettingAsync(userId.ToString(), privacySettingDto);

    }
}
