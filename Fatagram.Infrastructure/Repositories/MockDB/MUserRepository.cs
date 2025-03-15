using Fatagram.Domain.Models;
using Fatagram.Domain.Utils;
using Fatagram.Infrastructure.Repositories.Interfaces;
using System.Diagnostics;

namespace Fatagram.Infrastructure.Repositories.MockDB
{
    public class MUserRepository : IUserRepository
    {
        public List<User> Users { get; set; }

        private readonly Dictionary<string, Func<User, string?>> fieldsMapping = new Dictionary<string, Func<User, string?>>
        {
            { Fields.UserId, user => user.Id.ToString() },
            { Fields.FirstName, user => user.FirstName },
            { Fields.LastName, user => user.LastName },
            { Fields.FullName, user => user.FullName },
            { Fields.Email, user => user.Email },
            { Fields.Phone, user => user.Phone },
            { Fields.Bio, user => user.Bio },
            { Fields.Avatar, user => user.Avatar }
        };
       
        public MUserRepository()
        {

            Users = new List<User>();
            
        }

        public Task<bool> CreateUserAsync(User newUser)
        {
            Users.Add(newUser);
            return Task.FromResult(true);
        }

        public Task<User?> GetUserByIdAsync(Guid id)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetUserByUsernameAsync(string username)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email == username));
        }

        public Task<bool> UpdateUserAsync(User updateUser)
        {
            var user = Users.FirstOrDefault(user => user.Id == updateUser.Id);
            if (user == null) return Task.FromResult(false);

            if (updateUser.LastName != null) user.LastName = updateUser.LastName;
            if (updateUser.FirstName != null) user.FirstName = updateUser.FirstName;
            if (updateUser.Email != null) user.Email = updateUser.Email;
            if (updateUser.Phone != null) user.Phone = updateUser.Phone;
            if (updateUser.Bio != null) user.Bio = updateUser.Bio;
            if (updateUser.Avatar != null) user.Avatar = updateUser.Avatar;

            user.FullName = user.FirstName + " " + user.LastName;

            return Task.FromResult(true);
        }


        public Task<User?> GetUserByIdAsync(string id) => GetUserByIdAsync(Guid.Parse(id));

        public Task<string?[]> GetUserInfosWithFieldsAsync(string userId, string[] fields)
        {
            var user = Users.FirstOrDefault(user => user.Id.ToString() == userId);
            if (user == null) return Task.FromResult(new string?[0]);

            var res = new List<string?>();
            foreach (var field in fields)
            {
                if (fieldsMapping.ContainsKey(field))
                {
                    res.Add(fieldsMapping[field](user));
                }
                else
                {
                    res.Add("UNKNOWN_FIELD");
                }
            }

            return Task.FromResult(res.ToArray());
        }

        public Task<string> GetUserInfoByField(Guid userId, string field)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserInfoByField(string userId, string field)
        {
            throw new NotImplementedException();
        }
    }
}
