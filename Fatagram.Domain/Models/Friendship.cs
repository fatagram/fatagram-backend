using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    [Table("friendship")]
    public class Friendship : BaseEntity
    {
        [Column("id", TypeName = "uuid")]
        [Key]
        public Guid Id { get; set; }

        [Column("user_1_id", TypeName = "uuid")]
        [Required]
        public Guid User1Id { get; set; }

        [Column("user_2_id", TypeName = "uuid")]
        [Required]
        public Guid User2Id { get; set; }

        // Navigation properties
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
    }
}
