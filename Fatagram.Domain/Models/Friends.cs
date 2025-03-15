using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Fatagram.Domain.Models
{
    public class Friends
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }
        
        [Column("user_1_id")]
        public Guid User1Id { get; set; }

        [Column("user_2_id")]
        public Guid User2Id { get; set; }

        [Column("make_friend_time")]
        public DateTime MakeFriendTime { get; set; }
    }
}
