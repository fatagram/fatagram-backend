using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Shared.Enums
{
    public enum ResponseStatusCode
    {
        /// <summary>
        /// Success
        /// </summary>
        Success = 200,

        /// <summary>
        /// Created
        /// </summary>
        Created = 201,

        /// <summary>
        /// No Content
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// Bad Request
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// Unauthorized
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Forbidden
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// Not Found
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// Internal Server Error
        /// </summary>
        InternalServerError = 500,
        
    }
}