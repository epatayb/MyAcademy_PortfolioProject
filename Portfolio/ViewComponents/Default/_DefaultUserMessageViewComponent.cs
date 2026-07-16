using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultUserMessageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
