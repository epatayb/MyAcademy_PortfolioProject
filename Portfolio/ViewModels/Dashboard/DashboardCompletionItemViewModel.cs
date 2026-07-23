namespace Portfolio.ViewModels.Dashboard
{
    public class DashboardCompletionItemViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";
    }
}