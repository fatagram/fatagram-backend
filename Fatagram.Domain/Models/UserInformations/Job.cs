using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models.UserInformations
{
    [Table("job")]
    public class Job
    {
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("localization_key", TypeName = "varchar(100)")]
        public string LocalizationKey { get; set; } = string.Empty;

        [Column("created_at", TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at", TypeName = "timestamptz")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<UserJob> Users { get; set; } = new List<UserJob>();
    }
}
