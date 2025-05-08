using Fatagram.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    /// <summary>
    /// Represents the privacy settings for a user's field.
    /// </summary>
    [Table("user_privacies")]
    public class UserPrivacy
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user privacy setting.
        /// </summary>
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [Column("user_id", TypeName = "uuid")]
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the field for which the privacy setting is applied.
        /// </summary>
        [Column("field", TypeName = "varchar(10)")]
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the privacy level of the specified field.
        /// </summary>
        [Column("level", TypeName = "varchar(10)")]
        public PrivacyLevel PrivacyLevel { get; set; }


        /// <summary>
        /// Gets or sets the user associated with the privacy setting.
        /// </summary>
        public User User { get; set; } = null!;
    }
}
