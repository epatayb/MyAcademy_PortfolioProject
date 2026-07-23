namespace Portfolio.ViewModels.Dashboard
{
    public class DashboardMessageViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MessagePreview { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsRead { get; set; }
    }
}
