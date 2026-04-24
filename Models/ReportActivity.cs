namespace DemoMvc.Models
{
    public class ReportActivity
    {
        public int ReportDetailId { get; set; }
        public ReportDetail? ReportDetail { get; set; }

        public int WeeklyActivityId { get; set; }
        public WeeklyActivity? WeeklyActivity { get; set; }
    }
}
