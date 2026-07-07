using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Email> Emails { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Friendship> Friendships { get; set; } = null!;
        public DbSet<FriendRequest> FriendRequests { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<Localized> Localizeds { get; set; } = null!;
        public DbSet<Conversation> Conversations { get; set; } = null!;
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<MessageMedia> MessageMedias { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<RoutePermission> RoutePermissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddUser();
            modelBuilder.AddAccount();
            modelBuilder.AddEmail();
            modelBuilder.AddRefreshToken();
            modelBuilder.AddFriendship();
            modelBuilder.AddNotification();
            modelBuilder.AddLanguage();
            modelBuilder.AddLocalized();
            modelBuilder.AddConversation();
            modelBuilder.AddRbac();
        }
    }
}
