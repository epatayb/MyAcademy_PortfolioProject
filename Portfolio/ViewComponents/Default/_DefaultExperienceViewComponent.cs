using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using System.Threading.Tasks;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultExperienceViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultExperienceViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var experiences = await _context.Experiences
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.StartYear)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return View(experiences);
        }
    }
}
