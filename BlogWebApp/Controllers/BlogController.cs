using Microsoft.AspNetCore.Mvc;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
