using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Response;
using Fatagram.API.Utils;
using Fatagram.Application.Validation;
using Fatagram.Shared.Enums;

namespace Fatagram.API.Middlewares
{
    public class Validation
    {
        public RequestDelegate Next { get; }
        public readonly ILogger<Validation> Logger;

        public Validation(RequestDelegate next, ILogger<Validation> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (ValidationException ex)
            {
                Logger.LogError($"Validation error: {ex.Message}");
                throw new AppValidationException(ex.Message, ex.Message);
            }
        }
    }
}
