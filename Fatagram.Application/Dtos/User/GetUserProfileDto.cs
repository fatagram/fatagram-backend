using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public record GetUserProfileDto
    {
        public Dictionary<string, object?>? Infos { get; set; }
        public bool? IsOwner { get; set; }
    }
}
