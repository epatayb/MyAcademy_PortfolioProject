using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class SkillController : Controller
    {
        private readonly AppDbContext _context;

        public SkillController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .Where(x=>x.IsActive)
                .OrderBy(x=>x.Name)
                .ToListAsync();

            return View(skills);
        }
    }
}
