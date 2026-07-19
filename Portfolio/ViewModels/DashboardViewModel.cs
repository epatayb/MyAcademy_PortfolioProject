using Portfolio.Data.Entities;

namespace Portfolio.ViewModels
{
    public class DashboardViewModel
    {
        public int ProjectCount { get; set; }
        public int TechStackCount { get; set; }
        public int MessageCount { get; set; }
        public int UnReadMessageCount { get; set; }

        public List<UserMessage> LatestMessages { get; set; } = [];
        public List<Project> Projects { get; set; } = [];
    }
}
