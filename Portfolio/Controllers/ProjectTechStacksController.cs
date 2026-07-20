using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.Data.Entities;
using Portfolio.ViewModels;
using System.Threading.Tasks;
using static Portfolio.ViewModels.ProjectTechStackAddViewModel;

namespace Portfolio.Controllers
{
    public class ProjectTechStacksController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectTechStacksController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var projectTechStacks = _context.ProjectTechStacks
                                        .AsNoTracking()
                                        .Where(x => x.TechStack.IsActive)
                                        .Include(p => p.Project)
                                        .Include(t => t.TechStack).ToList();
            return View(projectTechStacks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var projects = _context.Projects.ToList();
            var techStacks = _context.TechStacks
                                .Where(x => x.IsActive)
                                .ToList();

            ViewBag.projects = (from project in projects
                                select new SelectListItem
                                {
                                    Text = project.Name,
                                    Value = project.Id.ToString()
                                }).ToList();

            ViewBag.techStacks = (from techStack in techStacks
                                  select new SelectListItem
                                  {
                                      Text = techStack.Name,
                                      Value = techStack.Id.ToString()
                                  }).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProjectTechStack projectTechStack)
        {
            _context.ProjectTechStacks.Add(projectTechStack);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Add(int projectId)
        {
            var model = await PrepareViewModel(projectId);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProjectTechStackAddViewModel model)
        {
            if (!model.TechStackId.HasValue)
            {
                var invalidModel = await PrepareViewModel(model.ProjectId);

                if (invalidModel is null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(nameof(model.TechStackId), "Bir teknoloji seçmelisiniz.");

                return View(invalidModel);
            }

            var projectExists = await _context.Projects.AnyAsync(x => x.Id == model.ProjectId);

            var techStackExists = await _context.TechStacks.AnyAsync(x => x.Id == model.TechStackId.Value && x.IsActive);

            if (!projectExists || !techStackExists)
            {
                return NotFound();
            }

            var alreadyAdded = await _context.ProjectTechStacks
                .AnyAsync(x =>
                    x.ProjectId == model.ProjectId &&
                    x.TechStackId == model.TechStackId.Value);

            if (!alreadyAdded)
            {
                var lastSortOrder = await _context.ProjectTechStacks
                    .Where(x => x.ProjectId == model.ProjectId)
                    .MaxAsync(x => (int?)x.SortOrder) ?? 0;

                var projectTechStack = new ProjectTechStack
                {
                    ProjectId = model.ProjectId,
                    TechStackId = model.TechStackId.Value,
                    SortOrder = lastSortOrder + 1
                };

                _context.ProjectTechStacks.Add(projectTechStack);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                nameof(Add),
                new { projectId = model.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, int projectId)
        {
            var projectTechStacks = await _context.ProjectTechStacks
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var relation = projectTechStacks
                .FirstOrDefault(x => x.Id == id);

            if (relation is null)
            {
                return NotFound();
            }

            _context.ProjectTechStacks.Remove(relation);

            var remainingRelations = projectTechStacks
                .Where(x => x.Id != id)
                .ToList();

            for (var index = 0; index < remainingRelations.Count; index++)
            {
                remainingRelations[index].SortOrder = index + 1;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Add),
                new { projectId });
        }


        private async Task<ProjectTechStackAddViewModel?> PrepareViewModel(int projectId)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == projectId);
            if (project is null)
            {
                return null;
            }

            var addedTechStacks = await _context.ProjectTechStacks
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId && x.TechStack.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .Select(x => new AddedTechStackViewModel
                {
                    ProjectTechStackId = x.Id,
                    TechStackId = x.TechStackId,
                    Name = x.TechStack.Name,
                    SortOrder = x.SortOrder
                })
                .ToListAsync();

            var addedTechStackIds = addedTechStacks
                .Select(x => x.TechStackId)
                .ToList();

            var availableTechStacks = await _context.TechStacks
                .AsNoTracking()
                .Where(x => x.IsActive && !addedTechStackIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            return new ProjectTechStackAddViewModel
            {
                ProjectId = projectId,
                ProjectName = project.Name,

                AddedTechStacks = addedTechStacks,

                AvailableTechStacks = availableTechStacks
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList()
            };
        }


    }
}
