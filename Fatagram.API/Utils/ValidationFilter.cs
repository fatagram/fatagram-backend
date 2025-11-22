using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Shared.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fatagram.API.Utils
{
    public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            // For-each argument in action
            foreach (var arg in context.ActionArguments)
            {
                // If argument is null, skip it
                if (arg.Value == null)
                    continue;

                // Get IValidator<T> for argument type
                var type = typeof(IValidator<>).MakeGenericType(arg.Value.GetType());
                var validator = _serviceProvider.GetService(type);

                if (validator == null)
                    continue;

                // Get method IValidator<T>.ValidateAsync(T model, CancellationToken)
                var method =
                    type.GetMethod(
                        "ValidateAsync",
                        [arg.Value.GetType(), typeof(CancellationToken)]
                    ) ?? throw new Exception("[ValidationFilter] Validate Method not found.");

                // Execute method
                if (method.Invoke(validator, [arg.Value, CancellationToken.None]) is not Task task)
                    continue;
                await task.ConfigureAwait(false);

                var resultProp = task.GetType().GetProperty("Result");
                var validationResult = resultProp?.GetValue(task);

                var isValidProp = validationResult?.GetType().GetProperty("IsValid");
                if (isValidProp?.GetValue(validationResult) is not bool isValid || isValid)
                    continue;

                var errorsRaw = validationResult
                    ?.GetType()
                    .GetProperty("Errors")
                    ?.GetValue(validationResult);
                var errorList = (errorsRaw as IEnumerable<ValidationFailure>)
                    ?.Select(x => new Error(x.ErrorCode, x.ErrorMessage))
                    .ToList();

                if (!isValid)
                {
                    throw new ValidateException(errorList);
                }
            }

            await next();
        }
    }
}
