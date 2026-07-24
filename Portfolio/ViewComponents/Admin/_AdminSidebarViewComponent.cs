using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Admin
{
    public class _AdminSidebarViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            ViewBag.fullName = HttpContext.Session.GetString("fullName");
            return View();
        }
    }
}
