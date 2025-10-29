using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models.UserInformations
{
    [Table("school")]
    public class School : BaseEntity
    {
        [Column("id", TypeName = "uuid")]
        public Guid Id { get; set; }

        [Column("localization_key", TypeName = "varchar(100)")]
        public string LocalizationKey { get; set; } = string.Empty;

        // Navigation property
        public ICollection<UserSchool> Users { get; set; } = new List<UserSchool>();
    }
}
