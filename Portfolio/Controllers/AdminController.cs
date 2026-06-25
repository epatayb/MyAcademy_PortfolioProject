using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;

namespace Portfolio.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Layout()
        {
            return View();
        }
    }
}
