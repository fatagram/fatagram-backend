using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Auth
{
    public class GoogleCallbackDto
    {
        public string Code { get; set; } = string.Empty;
    }
}
