using Microsoft.AspNetCore.Mvc;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultHeadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }    
    }
}
