namespace Portfolio.ViewModels.Dashboard
{
    public class DashboardAlertViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = "warning";

        public string Type { get; set; } = "warning";

        public string Controller { get; set; } = string.Empty;

        public string Action { get; set; } = "Index";

        public int? RouteId { get; set; }
    }
}
