using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class MessageController : Controller
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.UserMessages
                .AsNoTracking()
                .OrderBy(x => x.IsRead)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();
            return View(messages);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var message = await _context.UserMessages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message is null)
            {
                return NotFound();
            }

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.UserMessages
                .FindAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            _context.UserMessages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var message = await _context.UserMessages
                .FindAsync(id);

            if (message is null)
            {
                return NotFound();
            }

            message.IsRead =false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
