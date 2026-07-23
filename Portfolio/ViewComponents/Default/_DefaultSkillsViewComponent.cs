using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using System.Threading.Tasks;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultSkillsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultSkillsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(skills);
        }
    }
}
