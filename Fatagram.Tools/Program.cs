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
        if (args.Length > 0 && args[0] == "fix-roles")
        {
            var adminServices = new ServiceCollection();
            ConfigureServices(adminServices, null);
            var adminProvider = adminServices.BuildServiceProvider();
            var db = adminProvider.GetRequiredService<AppDbContext>();

            var createGroupMessages = await db.Messages
                .Where(m => m.Type == MessageType.CreateGroup)
                .ToListAsync();

            var creatorByConvId = new Dictionary<Guid, Guid>();
            foreach (var m in createGroupMessages)
            {
                if (m.Metadata != null && m.Metadata.TryGetValue("creatorId", out var cidObj))
                {
                    if (Guid.TryParse(cidObj?.ToString(), out var cid))
                    {
                        creatorByConvId[m.ConversationId] = cid;
                    }
                }
                else if (m.SenderId.HasValue)
                {
                    creatorByConvId[m.ConversationId] = m.SenderId.Value;
                }
            }

            var conversations = await db.Conversations.Include(c => c.Participants).ToListAsync();
            int updated = 0;
            foreach (var c in conversations)
            {
                if (c.IsGroup)
                {
                    creatorByConvId.TryGetValue(c.Id, out var creatorGuid);

                    bool ownerFound = false;
                    foreach (var p in c.Participants)
                    {
                        if (creatorGuid != Guid.Empty && p.UserId == creatorGuid)
                        {
                            p.Role = ConversationRole.Owner;
                            ownerFound = true;
                        }
                        else
                        {
                            p.Role = ConversationRole.Member;
                        }
                    }

                    if (!ownerFound && c.Participants.Count > 0)
                    {
                        c.Participants.First().Role = ConversationRole.Owner;
                    }
                    updated++;
                }
                else
                {
                    foreach (var p in c.Participants)
                        p.Role = ConversationRole.Member;
                    updated++;
                }
            }
            // Sync LastSeenNumber for the actors/senders so ghost unread badges disappear
            var allMessages = await db.Messages.OrderBy(m => m.SequenceNumber).ToListAsync();
            var lastMessageByConv = allMessages
                .GroupBy(m => m.ConversationId)
                .ToDictionary(g => g.Key, g => g.Last());

            foreach (var c in conversations)
            {
                if (lastMessageByConv.TryGetValue(c.Id, out var lastMsg))
                {
                    Guid? actorId = lastMsg.SenderId;
                    if (actorId == null && lastMsg.Metadata != null)
                    {
                        if (lastMsg.Metadata.TryGetValue("actorId", out var aObj) && Guid.TryParse(aObj?.ToString(), out var aGuid))
                            actorId = aGuid;
                        else if (lastMsg.Metadata.TryGetValue("creatorId", out var cObj) && Guid.TryParse(cObj?.ToString(), out var cGuid))
                            actorId = cGuid;
                    }

                    if (actorId.HasValue)
                    {
                        var participant = c.Participants.FirstOrDefault(p => p.UserId == actorId.Value);
                        if (participant != null && participant.LastSeenNumber < lastMsg.SequenceNumber)
                        {
                            participant.LastSeenNumber = lastMsg.SequenceNumber;
                        }
                    }
                }
            }

            await db.SaveChangesAsync();
            Console.WriteLine($"✅ Accurately updated roles and synced read status for {updated} conversations!");
            return;
        }

        if (args.Length > 0 && args[0] == "test-login")
        {
            var adminServices = new ServiceCollection();
            ConfigureServices(adminServices, null);
            var adminProvider = adminServices.BuildServiceProvider();
            var db = adminProvider.GetRequiredService<AppDbContext>();

            var usernameOrEmail = args.Length > 1 ? args[1] : "admin";
            var password = args.Length > 2 ? args[2] : "Admin@123456";

            Console.WriteLine($"Testing query for '{usernameOrEmail}'...");
            var account = await db
                .Accounts.Include(a => a.User)
                .Where(a =>
                    a.Username == usernameOrEmail
                    || a.User.UserEmails.Any(ue =>
                        ue.Email.Address == usernameOrEmail && ue.IsVerified
                    )
                )
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (account == null)
            {
                Console.WriteLine("❌ Account not found!");
                return;
            }

            Console.WriteLine(
                $"✅ Account found: Id={account.Id}, UserId={account.UserId}, Username={account.Username}, IsActive={account.IsActive}"
            );
            Console.WriteLine($"PasswordHash: {account.PasswordHash}");
            var verify = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
            Console.WriteLine($"Password match: {verify}");

            var adminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var userRoles = await db
                .UserRoles.Where(ur => ur.UserId == account.UserId)
                .ToListAsync();
            Console.WriteLine($"UserRoles count: {userRoles.Count}");
            foreach (var ur in userRoles)
            {
                Console.WriteLine($"  - RoleId: {ur.RoleId}, ResourceId: {ur.ResourceId}");
            }

            var query = db
                .UserRoles.Where(ur => ur.UserId == account.UserId && ur.ResourceId == null)
                .SelectMany(ur => ur.Role.Permissions.Select(p => p.Name));
            var perms = await query.Distinct().ToListAsync();
            Console.WriteLine($"Permissions count: {perms.Count}");
            foreach (var p in perms)
            {
                Console.WriteLine($"  - Perm: {p}");
            }

            return;
        }

        if (args.Length > 0 && args[0] == "admin-seed")
        {
            Console.WriteLine("=== Fatagram Admin User Creator Tool ===\n");
            var username = args.Length > 1 ? args[1] : "admin";
            var password = args.Length > 2 ? args[2] : "Admin@123456";
            var emailAddr = args.Length > 3 ? args[3] : "admin@fatagram.com";

            var adminServices = new ServiceCollection();
            ConfigureServices(adminServices, null);
            var adminProvider = adminServices.BuildServiceProvider();
            var db = adminProvider.GetRequiredService<AppDbContext>();

            var adminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");

            var existingAccount = await db
                .Accounts.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Username == username);
            if (existingAccount != null)
            {
                existingAccount.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                existingAccount.IsActive = true;
                existingAccount.UpdatedAt = DateTime.UtcNow;

                var userRole = await db.UserRoles.FirstOrDefaultAsync(ur =>
                    ur.UserId == existingAccount.UserId && ur.RoleId == adminRoleId
                );
                if (userRole == null)
                {
                    db.UserRoles.Add(
                        new UserRole
                        {
                            Id = Guid.NewGuid(),
                            UserId = existingAccount.UserId,
                            RoleId = adminRoleId,
                            CreatedAt = DateTime.UtcNow,
                        }
                    );
                }

                await db.SaveChangesAsync();
                Console.WriteLine(
                    $"\n✅ Admin account '{username}' updated successfully with Admin role!"
                );
            }
            else
            {
                var userId = Guid.NewGuid();
                var accountId = Guid.NewGuid();
                var emailId = Guid.NewGuid();

                var user = new User
                {
                    Id = userId,
                    UrlName = username,
                    FirstName = "System",
                    LastName = "Administrator",
                    FullName = "System Administrator",
                    Gender = Gender.Other,
                    BirthDay = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Avatar = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150",
                    Bio = "System Administrator for Fatagram",
                    IsOnBoarding = true,
                    LanguageCode = "en",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var account = new Account
                {
                    Id = accountId,
                    UserId = userId,
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var email = new Email
                {
                    Id = emailId,
                    Address = emailAddr,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var userEmail = new UserEmail
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EmailId = emailId,
                    IsPrimary = true,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var userRole = new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = adminRoleId,
                    CreatedAt = DateTime.UtcNow,
                };

                db.Users.Add(user);
                db.Accounts.Add(account);
                db.Emails.Add(email);
                db.Set<UserEmail>().Add(userEmail);
                db.UserRoles.Add(userRole);

                await db.SaveChangesAsync();
                Console.WriteLine(
                    $"\n✅ Admin account '{username}' created successfully with Admin role!"
                );
            }

            Console.WriteLine("\n--- Admin Credentials ---");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Email: {emailAddr}");
            Console.WriteLine($"Role: Admin ({adminRoleId})");
            return;
        }

        Console.WriteLine("=== Fatagram Bulk Account Creator & Friend Request Tool ===\n");

        // Parse arguments
        if (args.Length < 2)
        {
            Console.WriteLine(
                "Usage: dotnet run <targetUserId> <numberOfAccounts> [connectionString]\n   or: dotnet run admin-seed [username] [password] [email]"
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
            connectionString = Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection"
            );

            if (string.IsNullOrEmpty(connectionString))
            {
                var candidatePaths = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), "Fatagram.API", ".env"),
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "Fatagram.API", ".env"),
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "..",
                        "..",
                        "..",
                        "..",
                        "Fatagram.API",
                        ".env"
                    ),
                };

                foreach (var envPath in candidatePaths)
                {
                    if (File.Exists(envPath))
                    {
                        foreach (var line in File.ReadAllLines(envPath))
                        {
                            if (line.StartsWith("ConnectionStrings__DefaultConnection="))
                            {
                                connectionString = line.Substring(
                                        "ConnectionStrings__DefaultConnection=".Length
                                    )
                                    .Trim();
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(connectionString))
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                connectionString =
                    config.GetConnectionString("DefaultConnection")
                    ?? "Host=localhost;Port=5456;Database=fatagram;Username=postgres;Password=FatPro@123";
            }
        }

        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));
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
            var emailId = Guid.NewGuid();
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
                Id = emailId,
                Address = emailAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var userEmail = new UserEmail
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EmailId = emailId,
                IsPrimary = true,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _dbContext.Users.Add(user);
            _dbContext.Accounts.Add(account);
            _dbContext.Emails.Add(email);
            _dbContext.Set<UserEmail>().Add(userEmail);
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
