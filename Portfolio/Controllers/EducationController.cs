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
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return View(educations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Education());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Education model)
        {
            NormalizeEducation(model);

            if (!model.IsCurrent &&
                string.IsNullOrWhiteSpace(model.GraduationYear))
            {
                ModelState.AddModelError(
                    nameof(model.GraduationYear),
                    "Eğitim tamamlandıysa mezuniyet yılı girilmelidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.IsActive = true;

            _context.Educations.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var education = await _context.Educations
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (education is null)
            {
                return NotFound();
            }

            return View(education);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Education model)
        {
            NormalizeEducation(model);

            if (!model.IsCurrent &&
                string.IsNullOrWhiteSpace(model.GraduationYear))
            {
                ModelState.AddModelError(
                    nameof(model.GraduationYear),
                    "Eğitim tamamlandıysa mezuniyet yılı girilmelidir.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var education = await _context.Educations
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive);

            if (education is null)
            {
                return NotFound();
            }

            education.SchoolName = model.SchoolName;
            education.Department = model.Department;
            education.Description = model.Description;
            education.GPA = model.GPA;
            education.StartYear = model.StartYear;
            education.GraduationYear = model.GraduationYear;
            education.IsCurrent = model.IsCurrent;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var education = await _context.Educations
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

            if (education is null)
            {
                return NotFound();
            }

            education.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }        

        private static void NormalizeEducation(Education model)
        {
            model.SchoolName = model.SchoolName?.Trim() ?? string.Empty;
            model.Department = model.Department?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.StartYear = model.StartYear?.Trim() ?? string.Empty;

            if (model.IsCurrent)
            {
                model.GraduationYear = null;
            }
            else
            {
                model.GraduationYear = string.IsNullOrWhiteSpace(model.GraduationYear)
                    ? null
                    : model.GraduationYear.Trim();
            }
        }

    }
}
