using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1.Admin
{
    [Route("api/v{version:apiVersion}/admin/ping")]
    public class AdminAuthController : AdminBaseController
    {
        /// <summary>
        /// Ping admin endpoint to verify admin access
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
