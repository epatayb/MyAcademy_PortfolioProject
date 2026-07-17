using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultFooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
