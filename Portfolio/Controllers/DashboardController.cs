using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.ViewModels;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                ProjectCount = await _context.Projects.CountAsync(),
                TechStackCount = await _context.TechStacks.CountAsync(),
                MessageCount = await _context.UserMessages.CountAsync(),
                UnReadMessageCount = await _context.UserMessages
                    .CountAsync(x=> !x.IsRead),

                LatestMessages = await _context.UserMessages
                    .AsNoTracking()
                    .OrderByDescending(x=> x.Id)
                    .Take(5)
                    .ToListAsync(),

                Projects = await _context.Projects
                    .AsNoTracking()
                    .OrderBy(x=> x.DisplayOrder)
                    .Take(5)
                    .ToListAsync()
            };
            return View();
        }
    }
}
