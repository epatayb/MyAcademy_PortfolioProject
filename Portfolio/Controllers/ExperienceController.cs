using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;

namespace Portfolio.Controllers
{
    public class ExperienceController : Controller
    {
        private readonly AppDbContext _context;

        public ExperienceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var experiences = await _context.Experiences
                .AsNoTracking()
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return View(experiences);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Experience());
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Experience model)
        {
            NormalizeExperience(model);

            if (!model.IsCurrent &&
                string.IsNullOrWhiteSpace(model.EndYear))
            {
                ModelState.AddModelError(
                    nameof(model.EndYear),
                    "Deneyim tamamlandıysa bitiş yılı girilmelidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.IsActive = true;

            _context.Experiences.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
                
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (experience is null)
            {
                return NotFound();
            }

            return View(experience);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Experience model)
        {
            NormalizeExperience(model);

            if (!model.IsCurrent &&
                string.IsNullOrWhiteSpace(model.EndYear))
            {
                ModelState.AddModelError(
                    nameof(model.EndYear),
                    "Deneyim tamamlandıysa bitiş yılı girilmelidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id);

            if (experience is null)
            {
                return NotFound();
            }

            experience.Title = model.Title;
            experience.Company = model.Company;
            experience.Description = model.Description;
            experience.StartYear = model.StartYear;
            experience.EndYear = model.EndYear;
            experience.IsCurrent = model.IsCurrent;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x => x.Id == id);

            if (experience is null)
            {
                return NotFound();
            }

            experience.IsActive = !experience.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeExperience(Experience model)
        {
            model.Title = model.Title?.Trim() ?? string.Empty;
            model.Company = model.Company?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.StartYear = model.StartYear?.Trim() ?? string.Empty;

            if (model.IsCurrent)
            {
                model.EndYear = null;
            }
            else
            {
                model.EndYear =
                    string.IsNullOrWhiteSpace(model.EndYear)
                        ? null
                        : model.EndYear.Trim();
            }
        }
    }
}