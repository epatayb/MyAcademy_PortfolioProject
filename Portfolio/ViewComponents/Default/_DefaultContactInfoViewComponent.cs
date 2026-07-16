using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultContactInfoViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultContactInfoViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var contactInfos = _context.ContactInfos.ToList();

            return View(contactInfos);
        }
    }
}
