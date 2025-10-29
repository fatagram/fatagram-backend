using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class Language : BaseEntity
    {
        [Key]
        [Column("code", TypeName = "varchar(2)")]
        public string Code { get; set; } = string.Empty;

        [Column("name", TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        public ICollection<NotificationContent>? NotificationContents { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
