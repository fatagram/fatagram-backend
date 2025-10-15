using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;

namespace Fatagram.API.Utils
{
    public static class ActionResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                var data = ApiResponse<T>.Success(result.Data, result.Message);
                return result.Code switch
                {
                    ResponseStatusCode.Success => new OkObjectResult(data),
                    ResponseStatusCode.Created => new CreatedResult(string.Empty, data),
                    _ => new OkObjectResult(data),
                };
            }
            else
            {
                var error = new ApiError()
                {
                    Code = result.ErrorCode,
                    Message = result.ErrorMessage,
                };
                var apiResponse = ApiResponse<T>.Failure(error);
                return result.Code switch
                {
                    ResponseStatusCode.BadRequest => new BadRequestObjectResult(apiResponse),
                    ResponseStatusCode.Unauthorized => new UnauthorizedObjectResult(apiResponse),
                    ResponseStatusCode.Forbidden => new ForbidResult(),
                    ResponseStatusCode.NotFound => new NotFoundObjectResult(apiResponse),
                    _ => new ObjectResult(apiResponse) { StatusCode = (int)result.Code },
                };
            }
        }
    }
}
