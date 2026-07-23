namespace Portfolio.ViewModels.Statistics
{
    public class StatisticInsightViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = "insights";

        public string Type { get; set; } = "primary";

        public string? Controller { get; set; }

        public string Action { get; set; } = "Index";

        public bool UseProjectIdRoute { get; set; }

        public int? RouteId { get; set; }
    }
}
