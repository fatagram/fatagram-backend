using Fatagram.API.Middlewares;

namespace Fatagram.API.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseMiddleware<GlobalException>();
        }
    }
}
