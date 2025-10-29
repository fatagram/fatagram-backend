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
    public class GlobalException
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalException> _logger;

        public GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
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
            var response = ApiError.Create();
            if (ex is AppException appException)
            {
                response = ApiError.Create(
                    code: appException.ErrorCode,
                    message: appException.Message,
                    codes: appException.ErrorCodes
                );

                context.Response.StatusCode = ex switch
                {
                    ValidateException => (int)HttpStatusCode.BadRequest,
                    BadRequestException => (int)HttpStatusCode.BadRequest,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError,
                };
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = ApiError.Create(
                    code: "INTERNAL_SERVER_ERROR",
                    message: "An error occured while processing your request. Please try again later."
                );
            }
            await context.Response.WriteAsync(response.ToString());
        }
    }
}
