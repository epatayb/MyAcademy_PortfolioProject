using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
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
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return View(skills);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Skill());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Skill model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingSkill = await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == model.Name.ToLower());

            if (existingSkill is not null)
            {
                if (existingSkill.IsActive)
                {
                    ModelState.AddModelError(nameof(model.Name),
                        "Bu yetenek zaten kayıtlıdır.");

                    return View(model);
                }

                existingSkill.IsActive = true;
                existingSkill.Name = model.Name;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Daha önce kaldırılan yetenek yeniden aktifleştirildi.";

                return RedirectToAction(nameof(Index));
            }

            model.IsActive = true;

            _context.Skills.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
               "Yetenek başarıyla eklendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (skill is null)
            {
                return NotFound();
            }


            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Skill model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var skill = await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.IsActive);

            if(skill is null)
            {
                return NotFound();
            }

            var duplicateExists = await _context.Skills
                .AnyAsync(x =>
                    x.Id != model.Id &&
                    x.Name.ToLower() == model.Name.ToLower());

            if (duplicateExists)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Bu isimde başka bir yetenek bulunmaktadır.");

                return View(model);
            }

            skill.Name = model.Name;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Yetenek başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (skill is null)
            {
                return NotFound();
            }

            skill.IsActive = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Yetenek başarıyla kaldırıldı.";

            return RedirectToAction(nameof(Index));
        }

    }
}