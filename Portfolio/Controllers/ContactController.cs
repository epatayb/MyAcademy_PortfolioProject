using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var contact = await _context.ContactInfos
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return View(contact);
        }

        public async Task<IActionResult> Create()
        {
            var contactExists = await _context.ContactInfos.AnyAsync();

            if (contactExists)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(new ContactInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactInfo model)
        {
            var contactInfoExists = await _context.ContactInfos.AnyAsync();

            if (contactInfoExists)
            {
                return RedirectToAction(nameof(Index));
            }

            NormalizeContactInfo(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _context.ContactInfos.AddAsync(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "İletişim bilgileri başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var contactInfo = await _context.ContactInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (contactInfo is null)
            {
                return NotFound();
            }

            return View(contactInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ContactInfo model)
        {
            NormalizeContactInfo(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contactInfo = await _context.ContactInfos
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (contactInfo is null)
            {
                return NotFound();
            }

            contactInfo.Email = model.Email;
            contactInfo.Address = model.Address;
            contactInfo.LinkedinUrl = model.LinkedinUrl;
            contactInfo.GithubUrl = model.GithubUrl;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "İletişim bilgileri başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        private static void NormalizeContactInfo(ContactInfo model)
        {
            model.Email = model.Email?.Trim() ?? string.Empty;
            model.Address = model.Address?.Trim() ?? string.Empty;
            model.LinkedinUrl = model.LinkedinUrl?.Trim() ?? string.Empty;
            model.GithubUrl = model.GithubUrl?.Trim() ?? string.Empty;
        }
    }
}