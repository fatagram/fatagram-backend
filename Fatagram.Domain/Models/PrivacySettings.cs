using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    public class PrivacySettings
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("field")]
        public string Field { get; set; } = "post";

        [Column("privacy_level")]
        public string PrivacyLevel { get; set; } = "public";
    }
}
