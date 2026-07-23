namespace Portfolio.ViewModels.Statistics
{
    public class TechnologyUsageStatisticViewModel
    {
        public int TechStackId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int ProjectCount { get; set; }

        public List<string> ProjectNames { get; set; } = [];
    }
}