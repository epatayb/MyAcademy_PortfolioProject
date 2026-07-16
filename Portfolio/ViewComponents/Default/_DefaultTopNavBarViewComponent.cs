using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultTopNavBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
