namespace Portfolio.ViewModels
{
    public class ProjectTechStackListViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string TechStacks { get; set; } = string.Empty;
    }
}