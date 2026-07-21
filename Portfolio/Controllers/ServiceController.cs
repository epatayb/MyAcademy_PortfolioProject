using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;

namespace Portfolio.Controllers
{
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Service());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service model)
        {
            NormalizeService(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _context.Services.AddAsync(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla eklendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var service = await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (service is null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Service model)
        {
            NormalizeService(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (service is null)
            {
                return NotFound();
            }

            service.Title = model.Title;
            service.Description = model.Description;
            service.Icon = model.Icon;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(x => x.Id == id);

            if (service is null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeService(Service model)
        {
            model.Title = model.Title?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.Icon = model.Icon?.Trim() ?? string.Empty;
        }
    }
}