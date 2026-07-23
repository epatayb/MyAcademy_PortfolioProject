namespace Portfolio.ViewModels.Statistics
{
    public class StatisticsViewModel
    {
        // Genel proje ve teknoloji  istatistikleri
        public int ProjectCount { get; set; }

        public int ActiveTechnologyCount { get; set; }

        public int TotalProjectTechnologyRelationCount { get; set; }

        public double AverageTechnologyPerProject { get; set; }

        public string MostUsedTechnologyName { get; set; } = "Veri yok";

        public int MostUsedTechnologyProjectCount { get; set; }

        public int ProjectWithoutTechnologyCount { get; set; }


        // Mesaj istatistikleri
        public int TotalMessageCount { get; set; }

        public int ReadMessageCount { get; set; }

        public int UnreadMessageCount { get; set; }

        public int LastSevenDaysMessageCount { get; set; }

        public double MessageReadRate { get; set; }


        // Proje bazlı teknoloji dağılımı
        public List<ProjectTechnologyStatisticViewModel>
            ProjectTechnologyStatistics
        { get; set; } = [];


        // Teknoloji kullanım sıralaması
        public List<TechnologyUsageStatisticViewModel>
            TechnologyUsageStatistics
        { get; set; } = [];


        // Son yedi günlük mesaj grafiği
        public List<DailyMessageStatisticViewModel>
            DailyMessageStatistics
        { get; set; } = [];


        // Aktif ve pasif içerikler
        public List<ContentStatusStatisticViewModel>
            ContentStatusStatistics
        { get; set; } = [];


        // Eğitim ve deneyim dağılımı
        public List<ResumeStatusStatisticViewModel>
            ResumeStatusStatistics
        { get; set; } = [];


        // Öne çıkan içgörüler
        public List<StatisticInsightViewModel>
            Insights
        { get; set; } = [];

    }
}
