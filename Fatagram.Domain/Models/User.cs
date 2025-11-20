using Fatagram.Domain.Enums;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string? UrlName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the middle name of the user.
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the bio of the user.
        /// </summary>
        public string? Bio { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL of the user.
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Gets or sets the background image URL of the user.
        /// </summary>
        public string? Background { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Gender of the user
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Gets or sets the language code of the user
        /// </summary>
        public string LanguageCode { get; set; } = "en";

        /// <summary>
        /// Gets or sets the description of the user.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the nickname of the user.
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has completed onboarding.
        /// </summary>
        public bool IsOnBoarding { get; set; }

        public Language? Language { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        // Friend requests
        public ICollection<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();

        // Friend requests received
        public ICollection<FriendRequest> FriendRequestsReceived { get; set; } =
            new List<FriendRequest>();

        // Friendships
        public ICollection<Friendship> FriendshipAsUser1 { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipAsUser2 { get; set; } = new List<Friendship>();

        public ICollection<UserNotification> UserNotifications { get; set; } =
            new List<UserNotification>();
    }
}
