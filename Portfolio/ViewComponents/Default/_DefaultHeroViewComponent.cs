using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultHeroViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultHeroViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var hero = _context.Banners.FirstOrDefault();
            return View(hero);
        }
    }
}
