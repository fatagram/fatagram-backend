
using Fatagram.API.Utils;
using Fatagram.Application.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Fatagram.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError($"An error occurred {ex}");

                await HanldeExceptionAsync(context, ex);
            }
        }

        private async Task HanldeExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            ApiResponse<object> response = ApiResponse<object>.Failure();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            if (ex is AppException appException)
            {
                response.Error = new ApiError()
                {
                    Code = appException.Code,
                    Message = appException.Message,
                };
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                if (appException is UnauthorizedException unauthorized)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else if (appException is ValidateException validateException)
                {
                    response.Error.Codes = validateException.Errors;
                }
                else if (appException is UserNotFoundException || appException is AccountNotFoundException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else if (appException is DuplicateException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Error = new ApiError()
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occured while processing your request. Please try again later.",
                };
            }
            
            await context.Response.WriteAsync(response.ToString());
        }
    }
}
