
using Fatagram.API.Utils;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
                // Color for log
                _logger.LogError($"An error occurred {ex}");

                await HanldeExceptionAsync(context, ex);
            }
        }

        private async Task HanldeExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            ApiResponse<object> response = ApiResponse<object>.Failure();

            if (ex is AppException appException)
            {
                response.Error = new ApiError()
                {
                    Code = appException.ErrorCode,
                    Message = appException.Message,
                    Codes = appException.ErrorCodes
                };

                context.Response.StatusCode = ex switch
                {
                    ValidateException => (int)HttpStatusCode.BadRequest,
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError
                };
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
