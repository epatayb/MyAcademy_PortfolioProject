using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Context;
using Portfolio.ViewModels.Statistics;

namespace Portfolio.Controllers
{
    public class StatisticController : Controller
    {
        private readonly AppDbContext _context;

        public StatisticController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new StatisticsViewModel();

            var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");

            /*
             * PROJELER VE PROJE TEKNOLOJİLERİ
             */

            var projects = await _context.Projects
                .AsNoTracking()
                .Include(x => x.ProjectTechStacks)
                .ThenInclude(x => x.TechStack)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();

            model.ProjectTechnologyStatistics = projects
                .Select(project =>
                {
                    var activeTechnologyRelations = project.ProjectTechStacks
                        .Where(x => x.TechStack.IsActive)
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.TechStack.Name)
                        .ToList();

                    return new ProjectTechnologyStatisticViewModel
                    {
                        ProjectId = project.Id,
                        ProjectName = project.Name,
                        DisplayOrder = project.DisplayOrder,

                        TechnologyCount = activeTechnologyRelations
                            .Select(x => x.TechStackId)
                            .Distinct()
                            .Count(),

                        TechnologyNames = activeTechnologyRelations
                            .Select(x => x.TechStack.Name)
                            .Distinct()
                            .ToList()
                    };
                })
                .OrderByDescending(x => x.TechnologyCount)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.ProjectName)
                .ToList();

            model.ProjectCount = model.ProjectTechnologyStatistics.Count;

            model.TotalProjectTechnologyRelationCount =
                model.ProjectTechnologyStatistics
                    .Sum(x => x.TechnologyCount);

            model.ProjectWithoutTechnologyCount =
                model.ProjectTechnologyStatistics
                    .Count(x => x.TechnologyCount == 0);

            model.AverageTechnologyPerProject =
                model.ProjectCount == 0
                    ? 0
                    : Math.Round(
                        model.TotalProjectTechnologyRelationCount /
                        (double)model.ProjectCount,
                        1);


            /*
             * TEKNOLOJİ KULLANIM İSTATİSTİKLERİ
             */

            var allTechnologies = await _context.TechStacks
                .AsNoTracking()
                .Include(x => x.ProjectTechStacks)
                .ThenInclude(x => x.Project)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var activeTechnologies = allTechnologies
                .Where(x => x.IsActive)
                .ToList();

            model.ActiveTechnologyCount = activeTechnologies.Count;

            model.TechnologyUsageStatistics = activeTechnologies
                .Select(technology =>
                {
                    var projectsUsingTechnology =
                        technology.ProjectTechStacks
                            .Select(x => new
                            {
                                x.ProjectId,
                                x.Project.Name
                            })
                            .Distinct()
                            .OrderBy(x => x.Name)
                            .ToList();

                    return new TechnologyUsageStatisticViewModel
                    {
                        TechStackId = technology.Id,
                        Name = technology.Name,
                        ProjectCount = projectsUsingTechnology.Count,

                        ProjectNames = projectsUsingTechnology
                            .Select(x => x.Name)
                            .ToList()
                    };
                })
                .OrderByDescending(x => x.ProjectCount)
                .ThenBy(x => x.Name)
                .ToList();

            var mostUsedTechnology =
                model.TechnologyUsageStatistics
                    .FirstOrDefault(x => x.ProjectCount > 0);

            model.MostUsedTechnologyName =
                mostUsedTechnology?.Name ?? "Veri yok";

            model.MostUsedTechnologyProjectCount =
                mostUsedTechnology?.ProjectCount ?? 0;


            /*
             * MESAJ İSTATİSTİKLERİ
             */

            var messages = await _context.UserMessages
                .AsNoTracking()
                .Select(x => new
                {
                    x.CreatedDate,
                    x.IsRead
                })
                .ToListAsync();

            model.TotalMessageCount = messages.Count;

            model.ReadMessageCount = messages
                .Count(x => x.IsRead);

            model.UnreadMessageCount = messages
                .Count(x => !x.IsRead);

            model.MessageReadRate =
                model.TotalMessageCount == 0
                    ? 0
                    : Math.Round(
                        model.ReadMessageCount * 100.0 /
                        model.TotalMessageCount,
                        1);


            /*
             * SON 7 GÜNLÜK MESAJ GRAFİĞİ
             */

            var today = DateTime.Today;
            var sevenDaysAgo = today.AddDays(-6);
            var tomorrow = today.AddDays(1);

            var lastSevenDaysMessages = messages
                .Where(x =>
                    x.CreatedDate >= sevenDaysAgo &&
                    x.CreatedDate < tomorrow)
                .ToList();

            model.LastSevenDaysMessageCount =
                lastSevenDaysMessages.Count;

            model.DailyMessageStatistics = Enumerable
                .Range(0, 7)
                .Select(dayOffset =>
                {
                    var date = sevenDaysAgo.AddDays(dayOffset);

                    return new DailyMessageStatisticViewModel
                    {
                        Date = date,

                        DayName = GetTurkishShortDayName(
                            date.DayOfWeek),

                        MessageCount = lastSevenDaysMessages
                            .Count(x =>
                                x.CreatedDate.Date == date.Date)
                    };
                })
                .ToList();


            /*
             * AKTİF VE PASİF İÇERİKLER
             */

            var skillStatuses = await _context.Skills
                .AsNoTracking()
                .Select(x => x.IsActive)
                .ToListAsync();

            var educationStatuses = await _context.Educations
                .AsNoTracking()
                .Select(x => new
                {
                    x.IsActive,
                    x.IsCurrent
                })
                .ToListAsync();

            var experienceStatuses = await _context.Experiences
                .AsNoTracking()
                .Select(x => new
                {
                    x.IsActive,
                    x.IsCurrent
                })
                .ToListAsync();

            model.ContentStatusStatistics =
            [
                new ContentStatusStatisticViewModel
                {
                    Title = "Teknolojiler",
                    Icon = "terminal",

                    ActiveCount = allTechnologies
                        .Count(x => x.IsActive),

                    PassiveCount = allTechnologies
                        .Count(x => !x.IsActive),

                    Controller = "TechStack",
                    Action = "Index"
                },

                new ContentStatusStatisticViewModel
                {
                    Title = "Yetenekler",
                    Icon = "psychology",

                    ActiveCount = skillStatuses
                        .Count(x => x),

                    PassiveCount = skillStatuses
                        .Count(x => !x),

                    Controller = "Skill",
                    Action = "Index"
                },

                new ContentStatusStatisticViewModel
                {
                    Title = "Eğitimler",
                    Icon = "school",

                    ActiveCount = educationStatuses
                        .Count(x => x.IsActive),

                    PassiveCount = educationStatuses
                        .Count(x => !x.IsActive),

                    Controller = "Education",
                    Action = "Index"
                },

                new ContentStatusStatisticViewModel
                {
                    Title = "Deneyimler",
                    Icon = "work_history",

                    ActiveCount = experienceStatuses
                        .Count(x => x.IsActive),

                    PassiveCount = experienceStatuses
                        .Count(x => !x.IsActive),

                    Controller = "Experience",
                    Action = "Index"
                }
            ];


            /*
             * EĞİTİM VE DENEYİM DAĞILIMI
             *
             * Yalnızca aktif kayıtlar bu dağılıma dahil edilir.
             */

            model.ResumeStatusStatistics =
            [
                new ResumeStatusStatisticViewModel
                {
                    Title = "Eğitim Durumu",
                    Icon = "school",

                    CurrentCount = educationStatuses
                        .Count(x =>
                            x.IsActive &&
                            x.IsCurrent),

                    CompletedCount = educationStatuses
                        .Count(x =>
                            x.IsActive &&
                            !x.IsCurrent),

                    CurrentLabel = "Devam eden",
                    CompletedLabel = "Tamamlanan",

                    Controller = "Education",
                    Action = "Index"
                },

                new ResumeStatusStatisticViewModel
                {
                    Title = "Deneyim Durumu",
                    Icon = "work_history",

                    CurrentCount = experienceStatuses
                        .Count(x =>
                            x.IsActive &&
                            x.IsCurrent),

                    CompletedCount = experienceStatuses
                        .Count(x =>
                            x.IsActive &&
                            !x.IsCurrent),

                    CurrentLabel = "Devam eden",
                    CompletedLabel = "Tamamlanan",

                    Controller = "Experience",
                    Action = "Index"
                }
            ];


            /*
             * ÖNE ÇIKAN İÇGÖRÜLER
             */

            var mostComprehensiveProject =
                model.ProjectTechnologyStatistics
                    .OrderByDescending(x => x.TechnologyCount)
                    .ThenBy(x => x.ProjectName)
                    .FirstOrDefault();

            var leastComprehensiveProject =
                model.ProjectTechnologyStatistics
                    .OrderBy(x => x.TechnologyCount)
                    .ThenBy(x => x.ProjectName)
                    .FirstOrDefault();

            var usedTechnologyCount =
                model.TechnologyUsageStatistics
                    .Count(x => x.ProjectCount > 0);

            var technologyDiversityRate =
                model.ActiveTechnologyCount == 0
                    ? 0
                    : Math.Round(
                        usedTechnologyCount * 100.0 /
                        model.ActiveTechnologyCount,
                        1);

            var unusedTechnologyNames =
                model.TechnologyUsageStatistics
                    .Where(x => x.ProjectCount == 0)
                    .Select(x => x.Name)
                    .ToList();

            var busiestMessageDay = messages
                .GroupBy(x => x.CreatedDate.DayOfWeek)
                .Select(group => new
                {
                    DayOfWeek = group.Key,
                    MessageCount = group.Count()
                })
                .OrderByDescending(x => x.MessageCount)
                .ThenBy(x => (int)x.DayOfWeek)
                .FirstOrDefault();

            var lastInteractionDate = messages
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => (DateTime?)x.CreatedDate)
                .FirstOrDefault();

            model.Insights =
            [
                new StatisticInsightViewModel
                {
                    Title = "En Kapsamlı Proje",

                    Value = mostComprehensiveProject is null
                        ? "Veri yok"
                        : mostComprehensiveProject.ProjectName,

                    Description = mostComprehensiveProject is null
                        ? "Henüz proje eklenmedi."
                        : $"{mostComprehensiveProject.TechnologyCount} farklı teknoloji kullanılıyor.",

                    Icon = "deployed_code",
                    Type = "primary",

                    Controller = mostComprehensiveProject is null
                        ? null
                        : "Project",

                    Action = "Update",

                    RouteId = mostComprehensiveProject?.ProjectId
                },

                new StatisticInsightViewModel
                {
                    Title = "Teknoloji Çeşitliliği",

                    Value =
                        $"{usedTechnologyCount} / {model.ActiveTechnologyCount}",

                    Description = model.ActiveTechnologyCount == 0
                        ? "Henüz aktif teknoloji bulunmuyor."
                        : $"Aktif teknolojilerin %{technologyDiversityRate.ToString("0.#", turkishCulture)} kadarı projelerde kullanılıyor.",

                    Icon = "diversity_3",
                    Type = "success",

                    Controller = "TechStack",
                    Action = "Index"
                },

                new StatisticInsightViewModel
                {
                    Title = "Kullanılmayan Teknolojiler",

                    Value = unusedTechnologyNames.Count == 0
                        ? "Yok"
                        : $"{unusedTechnologyNames.Count} teknoloji",

                    Description = CreateUnusedTechnologyDescription(
                        unusedTechnologyNames),

                    Icon = unusedTechnologyNames.Count == 0
                        ? "task_alt"
                        : "link_off",

                    Type = unusedTechnologyNames.Count == 0
                        ? "success"
                        : "warning",

                    Controller = "ProjectTechStacks",
                    Action = "Index"
                },

                new StatisticInsightViewModel
                {
                    Title = "En Yoğun Mesaj Günü",

                    Value = busiestMessageDay is null
                        ? "Veri yok"
                        : GetTurkishFullDayName(
                            busiestMessageDay.DayOfWeek),

                    Description = busiestMessageDay is null
                        ? "Henüz iletişim mesajı bulunmuyor."
                        : $"Toplam {busiestMessageDay.MessageCount} mesaj gönderilmiş.",

                    Icon = "calendar_month",
                    Type = "purple",

                    Controller = "Message",
                    Action = "Index"
                },

                new StatisticInsightViewModel
                {
                    Title = "Son Etkileşim",

                    Value = lastInteractionDate.HasValue
                        ? lastInteractionDate.Value
                            .ToString(
                                "dd MMMM yyyy",
                                turkishCulture)
                        : "Veri yok",

                    Description = lastInteractionDate.HasValue
                        ? lastInteractionDate.Value
                            .ToString(
                                "HH:mm 'tarihinde son mesaj alındı.'",
                                turkishCulture)
                        : "Henüz iletişim mesajı bulunmuyor.",

                    Icon = "schedule",
                    Type = "neutral",

                    Controller = "Message",
                    Action = "Index"
                },

                new StatisticInsightViewModel
                {
                    Title = "En Az Teknoloji Kullanan Proje",

                    Value = leastComprehensiveProject is null
                        ? "Veri yok"
                        : leastComprehensiveProject.ProjectName,

                    Description = leastComprehensiveProject is null
                        ? "Henüz proje eklenmedi."
                        : leastComprehensiveProject.TechnologyCount == 0
                            ? "Bu projeye henüz teknoloji eklenmemiş."
                            : $"{leastComprehensiveProject.TechnologyCount} teknoloji kullanılıyor.",

                    Icon = "data_object",

                    Type = leastComprehensiveProject?.TechnologyCount == 0
                        ? "warning"
                        : "primary",

                    Controller = leastComprehensiveProject is null
                        ? null
                        : "ProjectTechStacks",

                    Action = "Add",

                    RouteId = leastComprehensiveProject?.ProjectId
                }
            ];

            return View(model);
        }

        private static string CreateUnusedTechnologyDescription(
            List<string> technologyNames)
        {
            if (technologyNames.Count == 0)
            {
                return "Bütün aktif teknolojiler en az bir projede kullanılıyor.";
            }

            if (technologyNames.Count <= 3)
            {
                return string.Join(", ", technologyNames) +
                       " henüz bir projeye bağlanmamış.";
            }

            var firstThreeTechnologies = technologyNames
                .Take(3);

            var remainingTechnologyCount =
                technologyNames.Count - 3;

            return string.Join(", ", firstThreeTechnologies) +
                   $" ve {remainingTechnologyCount} teknoloji daha kullanılmıyor.";
        }

        private static string GetTurkishShortDayName(
            DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Pzt",
                DayOfWeek.Tuesday => "Sal",
                DayOfWeek.Wednesday => "Çar",
                DayOfWeek.Thursday => "Per",
                DayOfWeek.Friday => "Cum",
                DayOfWeek.Saturday => "Cmt",
                DayOfWeek.Sunday => "Paz",
                _ => string.Empty
            };
        }

        private static string GetTurkishFullDayName(
            DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Pazartesi",
                DayOfWeek.Tuesday => "Salı",
                DayOfWeek.Wednesday => "Çarşamba",
                DayOfWeek.Thursday => "Perşembe",
                DayOfWeek.Friday => "Cuma",
                DayOfWeek.Saturday => "Cumartesi",
                DayOfWeek.Sunday => "Pazar",
                _ => string.Empty
            };
        }
    }
}