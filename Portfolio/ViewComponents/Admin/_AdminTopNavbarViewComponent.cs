using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Admin
{
    public class _AdminTopNavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
