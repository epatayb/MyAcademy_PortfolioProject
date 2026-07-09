using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Admin
{
    public class _AdminHeadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
