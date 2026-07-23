namespace Portfolio.ViewModels.Statistics
{
    public class ProjectTechnologyStatisticViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public int TechnologyCount { get; set; }

        public List<string> TechnologyNames { get; set; } = [];
    }
}