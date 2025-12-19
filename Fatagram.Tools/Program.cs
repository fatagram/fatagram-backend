using Bogus;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fatagram.Tools;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Fatagram Bulk Account Creator & Friend Request Tool ===\n");

        // Parse arguments
        if (args.Length < 2)
        {
            Console.WriteLine(
                "Usage: dotnet run <targetUserId> <numberOfAccounts> [connectionString]"
            );
            Console.WriteLine("Example: dotnet run 550e8400-e29b-41d4-a716-446655440000 50");
            return;
        }

        if (!Guid.TryParse(args[0], out var targetUserId))
        {
            Console.WriteLine("Error: Invalid target user ID format");
            return;
        }

        if (!int.TryParse(args[1], out var numberOfAccounts) || numberOfAccounts <= 0)
        {
            Console.WriteLine("Error: Number of accounts must be a positive integer");
            return;
        }

        string? connectionString = args.Length > 2 ? args[2] : null;

        // Setup DI and DbContext
        var services = new ServiceCollection();
        ConfigureServices(services, connectionString);
        var serviceProvider = services.BuildServiceProvider();

        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var tool = new BulkFriendRequestTool(dbContext);

        // Check target user exists
        Console.WriteLine($"Target User ID: {targetUserId}");
        Console.WriteLine($"Number of accounts to create: {numberOfAccounts}\n");

        var targetUser = await dbContext.Users.FindAsync(targetUserId);
        if (targetUser == null)
        {
            Console.WriteLine($"Error: Target user with ID {targetUserId} not found!");
            return;
        }

        Console.WriteLine($"Target user found: {targetUser.FullName} (@{targetUser.UrlName})\n");
        Console.WriteLine("Press ENTER to continue or CTRL+C to cancel...");
        Console.ReadLine();

        // Execute
        try
        {
            await tool.CreateAccountsAndSendFriendRequestsAsync(targetUserId, numberOfAccounts);
            Console.WriteLine("\n✅ Done!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static void ConfigureServices(IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            // Load from appsettings or use default
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            connectionString =
                config.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5456;Database=fatagram;Username=postgres;Password=FatPro@123";
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
    }
}

public class BulkFriendRequestTool
{
    private readonly AppDbContext _dbContext;
    private readonly Faker _faker;

    public BulkFriendRequestTool(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _faker = new Faker();
    }

    public async Task CreateAccountsAndSendFriendRequestsAsync(Guid targetUserId, int count)
    {
        Console.WriteLine($"Creating {count} accounts...\n");

        var createdUserIds = new List<Guid>();
        var batchSize = 100;

        for (int i = 0; i < count; i++)
        {
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var username = $"test_{accountId.ToString("N")[..8]}";
            var emailAddress = $"test_{Guid.NewGuid().ToString("N")[..8]}@test.com";
            var firstName = _faker.Name.FirstName();
            var lastName = _faker.Name.LastName();

            // Create User
            var user = new User
            {
                Id = userId,
                UrlName = username,
                FirstName = firstName,
                LastName = lastName,
                FullName = $"{firstName} {lastName}",
                Gender = _faker.PickRandom<Gender>(),
                BirthDay = _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
                Avatar = _faker.Internet.Avatar(),
                Bio = _faker.Lorem.Sentence(),
                IsOnBoarding = true,
                LanguageCode = "en",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Create Account
            var account = new Account
            {
                Id = accountId,
                UserId = userId,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"), // Default password
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            // Create Email
            var email = new Email
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Address = emailAddress,
                IsPrimary = true,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _dbContext.Users.Add(user);
            _dbContext.Accounts.Add(account);
            _dbContext.Emails.Add(email);
            createdUserIds.Add(userId);

            if ((i + 1) % batchSize == 0 || i == count - 1)
            {
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Created {i + 1}/{count} accounts...");
            }
        }

        Console.WriteLine($"\n✅ Created {count} accounts successfully!");
        Console.WriteLine($"\nSending friend requests to target user...\n");

        // Send friend requests
        var friendRequests = createdUserIds
            .Select(senderId => new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = targetUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            })
            .ToList();

        for (int i = 0; i < friendRequests.Count; i += batchSize)
        {
            var batch = friendRequests.Skip(i).Take(batchSize);
            _dbContext.FriendRequests.AddRange(batch);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine(
                $"Sent {Math.Min(i + batchSize, friendRequests.Count)}/{friendRequests.Count} friend requests..."
            );
        }

        Console.WriteLine($"\n✅ Sent {friendRequests.Count} friend requests successfully!");
        Console.WriteLine("\n--- Summary ---");
        Console.WriteLine($"Created accounts: {count}");
        Console.WriteLine($"Friend requests sent: {friendRequests.Count}");
        Console.WriteLine($"Default password for all accounts: Test@123");
    }
}
