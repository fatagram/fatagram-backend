using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.SignalR
{
    public static class SignalRExtensions
    {
        public static void AddSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
