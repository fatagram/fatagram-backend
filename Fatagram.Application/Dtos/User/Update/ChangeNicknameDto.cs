using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User.Update
{
    public record ChangeNicknameDto
    {
        public string Nickname { get; set; } = null!;
    }
}
