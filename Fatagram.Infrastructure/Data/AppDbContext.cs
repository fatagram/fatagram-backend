using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<NotificationContent> NotificationContents { get; set; }
        public DbSet<Localized> Localizeds { get; set; }

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
            modelBuilder.AddNotificationContent();
            modelBuilder.AddLocalized();
        }
    }
}
