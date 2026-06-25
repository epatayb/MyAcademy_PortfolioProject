using Microsoft.AspNetCore.Mvc;

namespace Portfolio.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
