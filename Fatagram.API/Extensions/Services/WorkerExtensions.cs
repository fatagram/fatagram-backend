using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services
{
    public static class WorkerExtensions
    {
        public static void AddWorkerServices(this IServiceCollection services)
        {
            services.AddHostedService<SyncSeenWorker>();
        }
    }
}
