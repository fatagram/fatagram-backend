using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Response;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;

namespace Fatagram.API.Utils
{
    public static class ActionResultExtension
    {
        public static IActionResult ToActionResult<TData>(this Result<TData> result)
        {
            object? data;
            if (result.IsSuccess)
            {
                data = ApiResponse<TData>.Create(result.Data, result.Message);
            }
            else
            {
                data = ApiError.Create(result.ErrorCode, result.ErrorMessage);
            }
            return CreateActionResult(result.Code, data);
        }

        public static IActionResult ToActionResult<TCursor, TItem>(
            this CursorPagedResult<TCursor, TItem> result
        )
        {
            object? data;
            if (result.IsSuccess)
            {
                data = CursorPaginatedApiResponse<TCursor, TItem>.Create(
                    result.Items,
                    result.NextCursor,
                    result.Total
                );
            }
            else
            {
                data = ApiError.Create(result.ErrorCode, result.ErrorMessage);
            }
            return CreateActionResult(result.Code, data);
        }

        public static IActionResult ToActionResult<TItem>(this PagedResult<TItem> result)
        {
            object? data;
            if (result.IsSuccess)
            {
                data = PaginatedApiResponse<TItem>.Create(
                    result.Items,
                    result.Total,
                    result.Page,
                    result.PageSize
                );
            }
            else
            {
                data = ApiError.Create(result.ErrorCode, result.ErrorMessage);
            }
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
