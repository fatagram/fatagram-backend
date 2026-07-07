using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public abstract class AdminBaseController : ControllerBase;
}
