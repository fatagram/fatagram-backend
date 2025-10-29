//using Fatagram.Domain.Models;
//using Fatagram.Domain.Utils;
//using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
//using Fatagram.Shared.Utils;
//using System.Diagnostics;

//namespace Fatagram.Infrastructure.Repositories.UserRepository
//{
//    public class MUserRepository : IUserRepository
//    {
//        public List<User> Users { get; set; }

//        //private readonly Dictionary<string, Func<User, string?>> fieldsMapping = new Dictionary<string, Func<User, string?>>
//        //{
//        //    { Fields.UserId, user => user.Id.ToString() },
//        //    { Fields.FirstName, user => user.FirstName },
//        //    { Fields.LastName, user => user.LastName },
//        //    { Fields.FullName, user => user.FullName },
//        //    { Fields.Email, user => user.Email },
//        //    { Fields.Phone, user => user.Phone },
//        //    { Fields.Bio, user => user.Bio },
//        //    { Fields.Avatar, user => user.Avatar },
//        //    { Fields.Background, user => user.Background },
//        //    { Fields.BirthDay, user => user.BirthDay.ToString() }
//        //};

//        public MUserRepository()
//        {

//            Users = new List<User>();

//        }

//        public Task<bool> CreateUserAsync(User newUser)
//        {
//            Users.Add(newUser);
//            return Task.FromResult(true);
//        }

//        public Task<User?> GetUserByIdAsync(Guid id)
//        {
//            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
//        }

//        public Task<User?> GetUserByIdAsync(string id) => GetUserByIdAsync(Guid.Parse(id));

//        public Task<User?> GetUserByUsernameAsync(string username)
//        {
//            return Task.FromResult(Users.FirstOrDefault(user => user.Email == username));
//        }

//        public Task<bool> UpdateUserAsync(User updateUser)
//        {
//            var user = Users.FirstOrDefault(user => user.Id == updateUser.Id);
//            if (user == null) return Task.FromResult(false);

//            if (updateUser.LastName != null) user.LastName = updateUser.LastName;
//            if (updateUser.FirstName != null) user.FirstName = updateUser.FirstName;
//            if (updateUser.Email != null) user.Email = updateUser.Email;
//            if (updateUser.Phone != null) user.Phone = updateUser.Phone;
//            if (updateUser.Bio != null) user.Bio = updateUser.Bio;
//            if (updateUser.Avatar != null) user.Avatar = updateUser.Avatar;

//            user.FullName = user.FirstName + " " + user.LastName;

//            return Task.FromResult(true);
//        }

//        Task<Result<User?>> IUserRepository.GetUserByUsernameAsync(string username)
//        {
//            throw new NotImplementedException();
//        }

//        Task<Result<User?>> IUserRepository.GetUserByIdAsync(Guid id)
//        {
//            throw new NotImplementedException();
//        }

//        Task<Result<User?>> IUserRepository.GetUserByIdAsync(string id)
//        {
//            throw new NotImplementedException();
//        }

//        Task<Result<string>> IUserRepository.CreateUserAsync(User newUser)
//        {
//            throw new NotImplementedException();
//        }

//        Task<Result<string>> IUserRepository.UpdateUserAsync(User updateUser)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
