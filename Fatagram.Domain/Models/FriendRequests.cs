using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fatagram.Domain.Models
{
    public class FriendRequests
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }
        
        [Column("sender_id")]
        public Guid SenderId { get; set; }

        [Column("receiver_id")]
        public Guid ReceiverId { get; set; }

        [Column("request_time")]
        public DateTime RequestTime { get; set; }
    }
}
