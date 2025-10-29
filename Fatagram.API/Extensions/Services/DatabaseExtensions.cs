using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Extensions.Services
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration Configuration
        )
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
