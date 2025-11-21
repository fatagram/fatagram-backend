using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Middlewares;

namespace Fatagram.API.Extensions.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CheckOnboardingMiddleware>();
        }
    }
}
