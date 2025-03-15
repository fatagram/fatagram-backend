using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.Interfaces;

namespace Fatagram.Infrastructure.Repositories.MockDB
{
    /// <summary>
    /// Mock account repository
    /// </summary>
    public class MAccountRepository : IAccountRepository
    {
        public List<Account> Accounts { get; set; }

        private readonly IUserRepository _userRepository;

        public MAccountRepository(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            Accounts = new List<Account>();

            var newUser1 = new User()
            {
                Id = Guid.NewGuid(),
                LastName = "Phat",
                FirstName = "Ngoc",
                FullName = "Ngoc Phat",
                Email = "ngocphat123@gmail.com"
            };

            var newAccount1 = new Account()
            {
                Id = Guid.NewGuid(),
                Username = "ngocphat123",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserId = newUser1.Id,
                User = newUser1
            };

            _userRepository.CreateUserAsync(newUser1);
            CreateAccountAsync(newAccount1);

            var newUser2 = new User()
            {
                Id = Guid.NewGuid(),
                LastName = "Tran",
                FirstName = "Minh",
                FullName = "Minh Tran",
                Email = "minhtran@mail.com"
            };

            var newAccount2 = new Account()
            {
                Id = Guid.NewGuid(),
                Username = "minhtran",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456@ppt"),
                UserId = newUser2.Id,
                User = newUser2
            };

            _userRepository.CreateUserAsync(newUser2);
            CreateAccountAsync(newAccount2);
        }

        public Task<bool> CreateAccountAsync(Account newAccount)
        {
            Accounts.Add(newAccount);
            return Task.FromResult(true);
        }

        public Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return Task.FromResult(Accounts.FirstOrDefault(account => account.Username == username));
        }

        public Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            return Task.FromResult(Accounts.FirstOrDefault(account => account.Id == accountId));
        }

        public Task<Account?> GetAccountByIdAsync(string accountId) => GetAccountByIdAsync(Guid.Parse(accountId));


        public Task<Account?> GetAccountByUserIdAsync(Guid userId)
        {
            return Task.FromResult(Accounts.FirstOrDefault(account => account.UserId == userId));
        }

        public Task<bool> UpdateAccountAsync(Account account)
        {
            var index = Accounts.FindIndex(a => a.Id == account.Id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }
            Accounts[index] = account;
            return Task.FromResult(true);
        }

        public Task<Account?> GetAccountByUserIdAsync(string userId) => GetAccountByUserIdAsync(Guid.Parse(userId));


    }
}
