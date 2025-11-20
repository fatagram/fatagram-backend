using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using Fatagram.API.Response;
using Fatagram.API.Utils;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

namespace Fatagram.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Color for log
                // _logger.LogError($"An error occurred {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            if (ex is AppException appException)
            {
                var response = ErrorResponse.Create(
                    code: appException.ErrorCode,
                    message: appException.Message,
                    errors: appException.ErrorMessages
                );

                context.Response.StatusCode = ex switch
                {
                    ValidateException => (int)HttpStatusCode.BadRequest,
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                await context.Response.WriteAsync(response.ToString());
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = ErrorResponse.Create(
                    code: "INTERNAL_SERVER_ERROR",
                    message: "An unexpected error occurred."
                );
                await context.Response.WriteAsync(response.ToString());
            }
        }
    }
}
