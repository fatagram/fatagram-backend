using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public class UserInfoOverview
    {
        public string? Bio { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public bool IsOwner { get; set; } = false;
    }
}
