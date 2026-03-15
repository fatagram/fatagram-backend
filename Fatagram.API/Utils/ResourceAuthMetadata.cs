using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Utils
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceAuth : Attribute
    {
        public string ResourceType { get; set; } = string.Empty;
        public string RouteKey { get; set; } = string.Empty;
    }
}
