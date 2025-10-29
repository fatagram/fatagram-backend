using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.DetailExceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fatagram.API.Utils
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context
                    .ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidateException(errors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No-op
        }
    }
}
