using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using Portfolio.ViewModels;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class TechStackController : Controller
    {
        private readonly AppDbContext _context;

        public TechStackController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var techStacks = await _context.TechStacks
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Include(x => x.ProjectTechStacks)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(techStacks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TechStackCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cleanName = model.Name.Trim();

            var existingTechStack = await _context.TechStacks
                .FirstOrDefaultAsync(x => x.Name.ToLower() == cleanName.ToLower());

            if (existingTechStack is not null)
            {
                if (existingTechStack.IsActive)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Bu teknoloji zaten kayıtlı.");

                    return View(model);
                }

                existingTechStack.IsActive = true;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var techStack = new TechStack
            {
                Name = cleanName,
                IsActive = true
            };

            _context.TechStacks.Add(techStack);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var techStack = await _context.TechStacks.FindAsync(id);

            if (techStack is null)
            {
                return NotFound();
            }

            var model = new TechStackEditViewModel
            {
                Id = techStack.Id,
                Name = techStack.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(TechStackEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var techStack = await _context.TechStacks.FindAsync(model.Id);

            if (techStack is null)
            {
                return NotFound();
            }

            var cleanName = model.Name.Trim();

            var isDuplicate = await _context.TechStacks
                .AnyAsync(x =>
                    x.Id != model.Id &&
                    x.Name.ToLower() == cleanName.ToLower());

            if (isDuplicate)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Bu isimde başka bir teknoloji zaten kayıtlı.");

                return View(model);
            }

            techStack.Name = cleanName;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var techStack = await _context.TechStacks.FindAsync(id);

            if (techStack is null)
            {
                return NotFound();
            }

            techStack.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}