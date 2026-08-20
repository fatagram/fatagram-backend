using Asp.Versioning;
using Fatagram.API.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [NotRequireOnBoarding]
    public abstract class AdminBaseController : ControllerBase;
}
