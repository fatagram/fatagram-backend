using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public abstract class BaseEntity
    {
        [Column("created_at", TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at", TypeName = "timestamptz")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at", TypeName = "timestamptz")]
        public DateTime? DeletedAt { get; set; }
    }
}
