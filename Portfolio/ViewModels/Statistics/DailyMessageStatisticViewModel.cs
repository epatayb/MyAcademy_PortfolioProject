namespace Portfolio.ViewModels.Statistics
{
    public class DailyMessageStatisticViewModel
    {
        public DateTime Date { get; set; }

        public string DayName { get; set; } = string.Empty;

        public int MessageCount { get; set; }
    }
}
