using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;

namespace Portfolio.Controllers
{
    public class BannerController : Controller
    {
        private readonly AppDbContext _context;

        public BannerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var banner = await _context.Banners
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return View(banner);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var bannerExists = await _context.Banners.AnyAsync();

            if (bannerExists)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new Banner());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Banner model)
        {
            var bannerExists = await _context.Banners.AnyAsync();

            if (bannerExists)
            {
                return RedirectToAction(nameof(Index));
            }

            NormalizeBanner(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Banners.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var banner = await _context.Banners
                .FindAsync(id);

            if (banner is null)
            {
                return NotFound();
            }

            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Banner model)
        {
            NormalizeBanner(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var banner = await _context.Banners
                .FindAsync(model.Id);

            if (banner is null)
            {
                return NotFound();
            }

            banner.Title = model.Title;
            banner.Description = model.Description;
            banner.ImageUrl = model.ImageUrl;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeBanner(Banner model)
        {
            model.Title = model.Title?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.ImageUrl = model.ImageUrl?.Trim() ?? string.Empty;
        }
    }
}