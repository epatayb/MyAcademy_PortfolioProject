
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class EducationController : Controller
    {
        private readonly AppDbContext _context;

        public EducationController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var educations = await _context.Educations
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.StartYear)
                .ToListAsync();

            return View(educations);
        }

        
    }
}
