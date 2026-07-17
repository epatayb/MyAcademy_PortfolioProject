using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultSkillsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultSkillsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var skills = _context.Skills.ToList();
            return View(skills);
        }
    }
}
