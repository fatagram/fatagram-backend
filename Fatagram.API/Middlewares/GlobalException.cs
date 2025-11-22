using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using Fatagram.API.Utils;
using Fatagram.API.Utils.Response;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

namespace Fatagram.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env
        )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            if (ex is AppException appException)
            {
                var response = ErrorResponse.Create(
                    new ErrorDetails()
                    {
                        Code = appException.Error.Code,
                        Detail = appException.Error.Message,
                    },
                    appException
                        .Errors?.Select(x => new ErrorDetails()
                        {
                            Code = x.Code,
                            Detail = x.Message,
                        })
                        .ToArray()
                );
                _logger.LogError(response.ToString());

                context.Response.StatusCode = ex switch
                {
                    ForbiddenException => (int)HttpStatusCode.Forbidden,
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
                var message = _env.IsDevelopment()
                    ? ex.ToString()
                    : "An unexpected error occurred.";
                var response = ErrorResponse.Create(
                    error: new ErrorDetails() { Code = "INTERNAL_SERVER_ERROR", Detail = message }
                );
                await context.Response.WriteAsync(response.ToString());
            }
        }
    }
}
