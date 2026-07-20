using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;
using System.Threading.Tasks;

namespace Portfolio.ViewComponents.Default
{
    public class _DefaultProjectViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public _DefaultProjectViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Include(x => x.ProjectTechStacks
                    .Where(y => y.TechStack.IsActive)
                    .OrderBy(y => y.SortOrder))
                    .ThenInclude(x => x.TechStack)
                    .OrderBy(x => x.DisplayOrder)
                    .ThenByDescending(x => x.Id)
                    .ToListAsync();
            return View(projects);
        }
    }
}