using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Domain.Models.UserInformations;
using Fatagram.Infrastructure.Data.Extensions;
using Fatagram.Infrastructure.Data.Extensions.UserInformations;
using Microsoft.EntityFrameworkCore;


namespace Fatagram.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserPrivacy> UserPrivacies { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<NotificationContent> NotificationContents { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<UserJob> UserJobs { get; set; } 
        public DbSet<School> Schools { get; set; }
        public DbSet<UserSchool> UserSchools { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Localized> Localizeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddUser();
            modelBuilder.AddAccount();
            modelBuilder.AddUserPrivacy();
            modelBuilder.AddRefreshToken();
            modelBuilder.AddFriend();
            modelBuilder.AddNotification();
            modelBuilder.AddLanguage();
            modelBuilder.AddNotificationContent();
            modelBuilder.AddLocalized();
            modelBuilder.AddJob();
            modelBuilder.AddSchool();
            modelBuilder.AddHobby();
            modelBuilder.AddSkill();
        }
    }
}
