using Microsoft.AspNetCore.Mvc;

namespace Fatagram.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Trả về Views/Home/Index.cshtml
        }
    }
}
