using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Include(x => x.ProjectTechStacks
                    .OrderBy(y => y.SortOrder))
                .ThenInclude(x => x.TechStack)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var lastOrder = await _context.Projects
                .Select(x => (int?)x.DisplayOrder)
                .MaxAsync() ?? 0;

            var project = new Project
            {
                DisplayOrder = lastOrder + 1
            };
            return View(project);
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            _context.Projects.Add(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        public IActionResult Update(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }
            _context.Projects.Update(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
