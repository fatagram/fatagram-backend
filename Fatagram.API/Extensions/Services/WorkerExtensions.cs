namespace Fatagram.API.Extensions.Services
{
    public static class WorkerExtensions
    {
        public static void AddWorkerServices(this IServiceCollection services)
        {
            // Background workers removed — messages and seen status are now written directly to DB.
            // Re-add workers here if batching is reintroduced in the future.
        }
    }
}
