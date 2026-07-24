using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    [AllowAnonymous]
    public class DefaultController : Controller
    {
        private readonly AppDbContext _context;

        public DefaultController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(UserMessage userMessage)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/Default/Index#contact");
            }

            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            TempData["MessageSent"] = true;

            return Redirect("/Default/Index#home");
        }
    }
}
