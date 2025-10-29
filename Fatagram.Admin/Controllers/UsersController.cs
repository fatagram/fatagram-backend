using System.Threading.Tasks;
using Fatagram.Application.Dtos.Account;
using Fatagram.Application.Services.AccountServices.Interface;
using Fatagram.Application.Services.UserServices;
using Fatagram.Application.Services.UserServices.Interface;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.AccountRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.Admin.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private int _pageSize = 9;

        public UsersController(
            IAccountService accountService,
            IUserService userService,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            ILogger<UsersController> logger
        )
        {
            _accountService = accountService;
            _userService = userService;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{page?}")]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
                page = 1;
            var accounts = await _accountService.GetAccountsAsync(page, _pageSize);
            ViewBag.Page = page;
            return View(accounts.Data);
        }

        [HttpPost]
        [Route("GenerateUsers")]
        public async Task<IActionResult> GenerateUsers([FromForm] int userCount)
        {
            _logger.LogInformation($"\n\nGenerating {userCount} users...\n\n");
            for (int i = 0; i < userCount; i++)
            {
                try
                {
                    var prefix = GeneratePrefixUsername();
                    var userId = Guid.NewGuid();
                    var newUser = new User()
                    {
                        Id = userId,
                        LastName = "User",
                        FirstName = "Test " + prefix,
                        FullName = "User Test " + prefix,
                        UrlName = "user" + prefix,
                        Email = "test" + prefix + "@gmail.com",
                    };
                    var account = new Account()
                    {
                        Username = "test" + prefix,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456789"),
                        UserId = userId,
                    };

                    await _userRepository.CreateUserAsync(newUser);
                    await _accountRepository.CreateAccountAsync(account);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating user: {ex.Message}");
                    continue;
                }
            }
            return RedirectToAction("Index");
        }

        public static string GeneratePrefixUsername()
        {
            Random _random = new Random();
            // Lấy ngày tháng năm hiện tại
            string date = DateTime.Now.ToString("MMdd");

            // Tạo một phần số ngẫu nhiên
            string number = _random.Next(1000, 9999).ToString();

            return date + number;
        }

        [HttpPost]
        [Route("AutoSendFriendRequest")]
        public async Task<IActionResult> AutoSendFriendRequest([FromForm] string targetId)
        {
            var targetAccount = await _accountRepository.GetAccountByIdAsync(targetId);
            if (targetAccount == null)
            {
                _logger.LogError($"Account with ID {targetId} not found.");
                return RedirectToAction("Index");
            }
            var targerUser = await _userRepository.GetUserByUsernameAsync(targetAccount.Username);
            if (targerUser == null)
            {
                _logger.LogError($"User with ID {targetId} not found.");
                return RedirectToAction("Index");
            }
            var data = await _accountRepository.GetAccountsAsync(1, 150, "test");
            foreach (var account in data.accounts)
            {
                if (account.UserId == targerUser.Id)
                {
                    continue;
                }
                try
                {
                    await _userService.SendAddFriendAsync(account.UserId, targerUser.Id);
                }
                catch (Exception)
                {
                    continue;
                }
                _logger.LogInformation(
                    $"Sent friend request from {account.Username} to {targerUser.FullName}"
                );
            }
            return RedirectToAction("Index");
        }
    }
}
