using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultServicesViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultServicesViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var services = _context.Services.ToList();
            return View(services);
        }
    }
}
