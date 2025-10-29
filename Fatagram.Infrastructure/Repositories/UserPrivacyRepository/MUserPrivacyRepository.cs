//using Fatagram.Domain.Enums;
//using Fatagram.Domain.Models;
//using Fatagram.Infrastructure.Data;
//using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
//using Fatagram.Shared.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Fatagram.Infrastructure.Repositories.UserPrivacyRepository
//{
//    public class MUserPrivacyRepository : IUserPrivacyRepository
//    {
//        public List<UserPrivacy> UserPrivacies { get; set; } = new List<UserPrivacy>();

//        public MUserPrivacyRepository()
//        {

//        }

//        public Task<Result<Dictionary<string, PrivacyLevel>>> GetPrivacyLevelsAsync(Guid userId, IEnumerable<string> fields)
//        {
//            try
//            {
//                var result = new Dictionary<string, PrivacyLevel>();
//                foreach (var field in fields)
//                {
//                    var userPrivacies = UserPrivacies.FirstOrDefault(up => up.UserId == userId && up.Field == field);
//                    if (userPrivacies == null)
//                    {
//                        result.Add(field, PrivacyLevel.Public);
//                    }
//                    else
//                    {
//                        result.Add(field, userPrivacies.PrivacyLevel);
//                    }
//                }
//                return Task.FromResult(Result<Dictionary<string, PrivacyLevel>>.Success(result));
//            }
//            catch (Exception)
//            {
//                return Task.FromResult(Result<Dictionary<string, PrivacyLevel>>.Failure());
//            }
//        }

//        public Task<Result<Dictionary<string, PrivacyLevel>>> GetPrivacyLevelsAsync(string userId, IEnumerable<string> fields)
//            => GetPrivacyLevelsAsync(Guid.Parse(userId), fields);

//        public Task<Result<bool>> SetPrivacyLevelAsync(Guid userId, string field, PrivacyLevel privacyLevel)
//        {
//            var userPrivacy = UserPrivacies.FirstOrDefault(up => up.UserId == userId && up.Field == field);
//            if (userPrivacy == null)
//            {
//                UserPrivacies.Add(new UserPrivacy
//                {
//                    Id = Guid.NewGuid(),
//                    UserId = userId,
//                    Field = field,
//                    PrivacyLevel = privacyLevel
//                });
//            }
//            else
//            {
//                userPrivacy.PrivacyLevel = privacyLevel;
//            }
//            return Task.FromResult(Result<bool>.Success(true));
//        }

//        public Task<Result<bool>> SetPrivacyLevelAsync(string userId, string field, PrivacyLevel privacyLevel)
//            => SetPrivacyLevelAsync(Guid.Parse(userId), field, privacyLevel);

//        public Task<Result<bool>> SetAllPublicAsync(Guid userId)
//        {
//            foreach (var prop in typeof(User).GetProperties())
//            {
//                UserPrivacies.Add(new UserPrivacy
//                {
//                    Id = Guid.NewGuid(),
//                    UserId = userId,
//                    Field = prop.Name,
//                    PrivacyLevel = PrivacyLevel.Private
//                });
//            }
//            return Task.FromResult(Result<bool>.Success(true));
//        }

//        public Task<Result<bool>> SetAllPublicAsync(string userId) => SetAllPublicAsync(Guid.Parse(userId));

//        public Task<Result<bool>> SetPrivacyLevelAsync(string userId, UserPrivacy userPrivacy) => SetPrivacyLevelAsync(Guid.Parse(userId), userPrivacy);

//        public Task<Result<bool>> SetPrivacyLevelAsync(Guid userId, UserPrivacy userPrivacy)
//        {
//            try
//            {
//                var userPrivacyExist = UserPrivacies.FirstOrDefault(up => up.UserId == userId && up.Field == userPrivacy.Field);
//                if (userPrivacyExist == null)
//                {
//                    userPrivacy.Id = Guid.NewGuid();
//                    userPrivacy.UserId = userId;
//                    UserPrivacies.Add(userPrivacy);
//                }
//                else
//                {
//                    userPrivacyExist.PrivacyLevel = userPrivacy.PrivacyLevel;
//                }
//                return Task.FromResult(Result<bool>.Success(true));
//            }
//            catch (Exception)
//            {
//                return Task.FromResult(Result<bool>.Failure());
//            }
//        }
//    }
//}
