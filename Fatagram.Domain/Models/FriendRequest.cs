using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    [Table("friend_request")]
    public class FriendRequest
    {
        [Column("id", TypeName = "uuid")]
        [Key]
        public Guid Id { get; set; }
        
        [Column("sender_id", TypeName = "uuid")]
        [Required]
        public Guid SenderId { get; set; }
        
        [Column("receiver_id", TypeName = "uuid")]
        [Required]
        public Guid ReceiverId { get; set; }
        
        [Column("created_at", TypeName = "timestamptz")]
        [Required]
        public DateTime CreatedAt { get; set; }

        #region Navigation Properties

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        #endregion
    }
}
