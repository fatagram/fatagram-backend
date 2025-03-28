using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public class GetUserProfileDto
    {
        public Dictionary<string, string?>? Infos { get; set; }
        public bool? IsOwner { get; set; }
    }
}
