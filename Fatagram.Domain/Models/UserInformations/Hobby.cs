using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models.UserInformations
{
    [Table("hobby")]
    public class Hobby : BaseEntity
    {
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("localization_key", TypeName = "varchar(100)")]
        public string LocalizationKey { get; set; } = string.Empty;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
