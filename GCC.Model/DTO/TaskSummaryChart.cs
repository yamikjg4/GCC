namespace GlobalCalendar.MVC.DTO
{
    public class TaskSummaryChart
    {
        public string DomainId { get; set; }
        public string CompanyName { get; set; }
        public int TotalTask { get; set; }
        public int IsApprove { get; set; }
        public int TotalTaskPendingNotDue { get; set; }
        public int TotalTaskPendingOverDue { get; set; }
    }
}
