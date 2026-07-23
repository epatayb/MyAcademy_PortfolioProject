namespace Portfolio.ViewModels.Statistics
{
    public class ResumeStatusStatisticViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public int CurrentCount { get; set; }

        public int CompletedCount { get; set; }

        public int TotalCount => CurrentCount + CompletedCount;

        public string CurrentLabel { get; set; } = "Devam ediyor";

        public string CompletedLabel { get; set; } = "Tamamlandı";

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";
    }
}
