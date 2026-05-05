using DemoMvc.Models;

namespace DemoMvc.ViewModels.Instructor
{
    public class SubmittedReportsPagedViewModel
    {
        public List<Report> Items { get; set; } = new();

        public string? SelectedJcfUserId { get; set; }

        public ReportStatus? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int FromItem { get; set; }

        public int ToItem { get; set; }

        public List<ApplicationUser> JcfOptions { get; set; } = new();
    }
}