using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Utils
{
    public class SocketMessage<TPayload>
        where TPayload : class
    {
        public string Event { get; set; } = string.Empty;
        public TPayload? Payload { get; set; }
    }
}
