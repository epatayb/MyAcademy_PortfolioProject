using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.ViewModels;
using Portfolio.ViewModels.Dashboard;
using System.Threading.Tasks;

namespace Portfolio.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            // ANA İSTATİSTİKLER

            model.ProjectCount = await _context.Projects
                .AsNoTracking()
                .CountAsync();

            model.TechnologyCount = await _context.TechStacks
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            model.UnReadMessageCount = await _context.UserMessages
                .AsNoTracking()
                .CountAsync(x => !x.IsRead);

            model.ServiceCount = await _context.Services
                .AsNoTracking()
                .CountAsync();

            model.SkillCount = await _context.Skills
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            model.EducationCount = await _context.Educations
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            model.ExperienceCount = await _context.Experiences
                .AsNoTracking()
                .CountAsync(x => x.IsActive);

            // TEK KAYITLI ALANLAR

            var bannerExists = await _context.Banners
                .AsNoTracking()
                .AnyAsync();

            var aboutExists = await _context.Abouts
                .AsNoTracking()
                .AnyAsync();

            var contactInfoExists = await _context.ContactInfos
                .AsNoTracking()
                .AnyAsync();

            // PORTFOLYO TAMAMLANMA DURUMU

            model.CompletionItems =
            [
                new DashboardCompletionItemViewModel
                {
                    Title = "Banner",
                    Description = bannerExists
                    ? "Ana sayfa bannerı hazır."
                    : "Ana sayfa bannerı henüz eklenmedi.",
                    Icon = "web_asset",
                    IsCompleted = bannerExists,
                    Controller = "Banner",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "Hakkımda",
                    Description = aboutExists
                        ? "Hakkımda bölümü hazır."
                        : "Hakkımda bilgisi henüz eklenmedi.",
                    Icon = "person",
                    IsCompleted = aboutExists,
                    Controller = "About",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "İletişim Bilgileri",
                    Description = contactInfoExists
                        ? "İletişim bilgileri hazır."
                        : "İletişim bilgileri henüz eklenmedi.",
                    Icon = "contact_mail",
                    IsCompleted = contactInfoExists,
                    Controller = "ContactInfo",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "Projeler",
                    Description = model.ProjectCount > 0
                        ? $"{model.ProjectCount} proje bulunuyor."
                        : "Henüz proje eklenmedi.",
                    Icon = "deployed_code",
                    IsCompleted = model.ProjectCount > 0,
                    Controller = "Project",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "Hizmetler",
                    Description = model.ServiceCount > 0
                        ? $"{model.ServiceCount} hizmet bulunuyor."
                        : "Henüz hizmet eklenmedi.",
                    Icon = "design_services",
                    IsCompleted = model.ServiceCount > 0,
                    Controller = "Service",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "Eğitim",
                    Description = model.EducationCount > 0
                        ? $"{model.EducationCount} eğitim kaydı bulunuyor."
                        : "Henüz eğitim bilgisi eklenmedi.",
                    Icon = "school",
                    IsCompleted = model.EducationCount > 0,
                    Controller = "Education",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "İş Deneyimi",
                    Description = model.ExperienceCount > 0
                        ? $"{model.ExperienceCount} deneyim kaydı bulunuyor."
                        : "Henüz iş deneyimi eklenmedi.",
                    Icon = "work_history",
                    IsCompleted = model.ExperienceCount > 0,
                    Controller = "Experience",
                    Action = "Index"
                },

                new DashboardCompletionItemViewModel
                {
                    Title = "Yetenekler",
                    Description = model.SkillCount >= 3
                        ? $"{model.SkillCount} aktif yetenek bulunuyor."
                        : "En az 3 yetenek eklemeniz önerilir.",
                    Icon = "code_blocks",
                    IsCompleted = model.SkillCount >= 3,
                    Controller = "Skill",
                    Action = "Index"
                }
            ];

            var completedItemCount = model.CompletionItems
                .Count(x => x.IsCompleted);

            model.CompletionPercentage = model.CompletionItems.Count == 0
                ? 0
                : (int)Math.Round(
                    completedItemCount * 100.0 /
                    model.CompletionItems.Count);


            // SON GELEN 5 MESAJ

            var recentMessageRecords = await _context.UserMessages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Email,
                    x.MessageBody,
                    x.CreatedDate,
                    x.IsRead
                })
                .ToListAsync();

            model.RecentMessages = recentMessageRecords
                .Select(x => new DashboardMessageViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    MessagePreview = CreatePreview(
                        x.MessageBody,
                        90),
                    CreatedDate = x.CreatedDate,
                    IsRead = x.IsRead
                })
                .ToList();

            // EN ÇOK KULLANILAN TEKNOLOJİLER

            var technologyUsageRecords = await _context.TechStacks
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new
                {
                    x.Name,

                    ProjectCount = x.ProjectTechStacks
                        .Select(y => y.ProjectId)
                        .Distinct()
                        .Count()
                })
                .Where(x => x.ProjectCount > 0)
                .OrderByDescending(x => x.ProjectCount)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToListAsync();

            model.TopTechnologies = technologyUsageRecords
                .Select(x => new DashboardTechnologyUsageViewModel
                {
                    Name = x.Name,
                    ProjectCount = x.ProjectCount
                })
                .ToList();

            // DİKKAT GEREKTİREN İÇERİKLER

            var projectWithoutTechnologyCount =
                            await _context.Projects
                                .AsNoTracking()
                                .CountAsync(project =>
                                    !project.ProjectTechStacks.Any(relation =>
                                        relation.TechStack.IsActive));

            if (model.UnReadMessageCount > 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Okunmamış mesajlar var",
                    Description =
                        $"{model.UnReadMessageCount} mesaj okunmayı bekliyor.",
                    Icon = "mark_email_unread",
                    Type = "info",
                    Controller = "Message",
                    Action = "Index"
                });
            }

            if (!bannerExists)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Banner eksik",
                    Description =
                        "Ana sayfanın banner içeriğini oluşturun.",
                    Icon = "web_asset",
                    Type = "warning",
                    Controller = "Banner",
                    Action = "Index"
                });
            }

            if (!aboutExists)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Hakkımda bilgisi eksik",
                    Description =
                        "Kendinizi tanıtan hakkımda içeriğini ekleyin.",
                    Icon = "person",
                    Type = "warning",
                    Controller = "About",
                    Action = "Index"
                });
            }

            if (!contactInfoExists)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "İletişim bilgileri eksik",
                    Description =
                        "E-posta ve sosyal medya bağlantılarınızı ekleyin.",
                    Icon = "contact_mail",
                    Type = "warning",
                    Controller = "ContactInfo",
                    Action = "Index"
                });
            }

            if (model.ProjectCount == 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Henüz proje eklenmedi",
                    Description =
                        "Portfolyonuzu göstermek için ilk projenizi ekleyin.",
                    Icon = "deployed_code",
                    Type = "danger",
                    Controller = "Project",
                    Action = "Create"
                });
            }
            else if (projectWithoutTechnologyCount > 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Teknolojisi olmayan projeler var",
                    Description =
                        $"{projectWithoutTechnologyCount} projeye henüz teknoloji eklenmemiş.",
                    Icon = "account_tree",
                    Type = "warning",
                    Controller = "ProjectTechStacks",
                    Action = "Index"
                });
            }

            if (model.ServiceCount == 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Hizmet bilgisi eksik",
                    Description =
                        "Sunduğunuz hizmetleri portfolyonuza ekleyin.",
                    Icon = "design_services",
                    Type = "warning",
                    Controller = "Service",
                    Action = "Create"
                });
            }

            if (model.EducationCount == 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Eğitim bilgisi eksik",
                    Description =
                        "Eğitim geçmişinizi portfolyonuza ekleyin.",
                    Icon = "school",
                    Type = "warning",
                    Controller = "Education",
                    Action = "Create"
                });
            }

            if (model.ExperienceCount == 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "İş deneyimi eksik",
                    Description =
                        "İş veya staj deneyimlerinizi ekleyin.",
                    Icon = "work_history",
                    Type = "warning",
                    Controller = "Experience",
                    Action = "Create"
                });
            }

            if (model.SkillCount < 3)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Yetenek sayısı düşük",
                    Description =
                        $"Şu anda {model.SkillCount} aktif yetenek bulunuyor. En az 3 yetenek eklemeniz önerilir.",
                    Icon = "work_history",
                    Type = "warning",
                    Controller = "Skill",
                    Action = "Index"
                });
            }

            if (model.Alerts.Count == 0)
            {
                model.Alerts.Add(new DashboardAlertViewModel
                {
                    Title = "Tüm temel içerikler hazır",
                    Description =
                        "Portfolyonuzda dikkat gerektiren temel bir eksik bulunmuyor.",
                    Icon = "task_alt",
                    Type = "success"
                });
            }

            return View(model);
        }

        private static string CreatePreview(
            string? content,
            int maximumLength)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return "Mesaj içeriği bulunmuyor.";
            }

            var normalizedContent = content.Trim();

            return normalizedContent.Length <= maximumLength
                ? normalizedContent
                : normalizedContent[..maximumLength] + "...";
        }

    }
}
