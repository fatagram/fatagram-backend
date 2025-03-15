using Fatagram.Domain.Models;
using Fatagram.Domain.Utils;
using Fatagram.Infrastructure.Repositories.Interfaces;

namespace Fatagram.Infrastructure.Repositories.MockDB
{
    public class MPrivacySettingRepository : IPrivarySettingRepository
    {
        public List<PrivacySettings> PrivacySettings { get; set; }

        public MPrivacySettingRepository()
        {
            PrivacySettings = new List<PrivacySettings>();
        }

        public Task<string> GetPrivacyLevelAsync(Guid userId, string field)
        {
            var res = PrivacySettings.FirstOrDefault(ps => ps.UserId == userId && ps.Field == field);
            if (res == null) return Task.FromResult("private");
            return Task.FromResult(res.PrivacyLevel);
        }

        public Task<string> GetPrivacyLevelAsync(string userId, string field) => GetPrivacyLevelAsync(Guid.Parse(userId), field);

        public Task<bool> SetPrivacySettingAsync(Guid userId, string field, string privacyLevel)
        {
            var res = PrivacySettings.FirstOrDefault(ps => ps.UserId == userId && ps.Field == field);
            if (res == null)
            {
                PrivacySettings.Add(new PrivacySettings
                {
                    UserId = userId,
                    Field = field,
                    PrivacyLevel = privacyLevel
                });
            }
            else
            {
                res.PrivacyLevel = privacyLevel;
            }
            return Task.FromResult(true);
        }

        public Task<bool> SetPrivacySettingAsync(string userId, string field, string privacyLevel) => SetPrivacySettingAsync(Guid.Parse(userId), field, privacyLevel);

        public Task<Dictionary<string, string>> GetAllPrivacySettingsAysnc(string userId)
        {
            var res = PrivacySettings.Where(ps => ps.UserId.ToString() == userId).ToDictionary(ps => ps.Field, ps => ps.PrivacyLevel);
            return Task.FromResult(res);
        }

        public Task<bool> InitialPrivacySettingForNewUserAsync(Guid userId)
        {
            var isUserExist = PrivacySettings.Any(ps => ps.UserId == userId);
            if (isUserExist) return Task.FromResult(false);

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = "post",
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.Email,
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.LastName,
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.FirstName,
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.FullName,
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = "friendList",
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.Phone,
                PrivacyLevel = "public"
            });

            PrivacySettings.Add(new PrivacySettings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Field = Fields.Bio,
                PrivacyLevel = "public"
            });

            return Task.FromResult(true);
        }


        public Task<bool> InitialPrivacySettingForNewUserAsync(string userId) => InitialPrivacySettingForNewUserAsync(Guid.Parse(userId));
    }
}
