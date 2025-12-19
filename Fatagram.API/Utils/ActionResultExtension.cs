using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils.Response;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;

namespace Fatagram.API.Utils
{
    public static class ActionResultExtension
    {
        public static IActionResult ToActionResult<TData>(this Result<TData> result)
        {
            var data = ApiResponse<TData>.Create(result.Data, result.Message);
            return CreateActionResult(result.Code, data);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            var data = ApiResponse<object>.Create(null, result.Message);
            return CreateActionResult(result.Code, data);
        }

        public static IActionResult ToActionResult<TCursor, TItem>(
            this CursorResult<TCursor, TItem> result
        )
            where TCursor : struct
        {
            var data = CursorResponse<TCursor, TItem>.Create(
                result.Data ?? Enumerable.Empty<TItem>(),
                result.NextCursor,
                result.HasNext,
                result.Message
            );
            return CreateActionResult(result.Code, data);
        }

        public static IActionResult ToActionResult<TItem>(this PagedResult<TItem> result)
        {
            var data = PaginatedResponse<TItem>.Create(
                result.Data ?? Enumerable.Empty<TItem>(),
                result.Page,
                result.PageSize,
                result.Total,
                result.Message
            );
            return CreateActionResult(result.Code, data);
        }

        private static IActionResult CreateActionResult(ResponseStatusCode code, object? data)
        {
            return code switch
            {
                ResponseStatusCode.Success => new OkObjectResult(data),
                ResponseStatusCode.Created => new CreatedResult(string.Empty, data),
                ResponseStatusCode.BadRequest => new BadRequestObjectResult(data),
                ResponseStatusCode.Unauthorized => new UnauthorizedObjectResult(data),
                ResponseStatusCode.Forbidden => new ForbidResult(),
                ResponseStatusCode.NotFound => new NotFoundObjectResult(data),
                _ => new ObjectResult(data) { StatusCode = (int)code },
            };
        }
    }
}
