using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class Friendship : BaseEntity
    {
        public Guid User1Id { get; set; }
        public Guid User2Id { get; set; }
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;
    }
}
