namespace Portfolio.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // Ana istatistikler
        public int ProjectCount { get; set; }
        public int TechnologyCount { get; set; }
        public int ServiceCount { get; set; }
        public int UnReadMessageCount { get; set; }

        // Küçük özet bilgiler
        public int SkillCount { get; set; }
        public int EducationCount { get; set; }
        public int ExperienceCount { get; set; }

        // Portfolyo tamamlama durumu
        public int CompletionPercentage { get; set; }
        
        public List<DashboardCompletionItemViewModel> CompletionItems { get; set; } = [];

        // Son gelen mesajlar
        public List<DashboardMessageViewModel> RecentMessages { get; set; } = [];

        // En çok kullanılan teknolojiler
        public List<DashboardTechnologyUsageViewModel> TopTechnologies { get; set; }
            = [];

        // Dikkat gerektiren içerikler
        public List<DashboardAlertViewModel> Alerts { get; set; }
            = [];
    }
}
