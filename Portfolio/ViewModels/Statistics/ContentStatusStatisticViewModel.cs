namespace Portfolio.ViewModels.Statistics
{
    public class ContentStatusStatisticViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public int ActiveCount { get; set; }

        public int PassiveCount { get; set; }

        public int TotalCount => ActiveCount + PassiveCount;

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";
    }
}
