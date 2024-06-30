namespace GlobalCalendar.MVC.DTO
{
    public class TaskSummaryViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Companyname { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int TotalTask { get; set; }
        public int TotalTaskCompleted { get; set; }
        public int TotalTaskPendingNotdue { get; set; }
        public int TotalTaskPendingOverdue { get; set; }
    }
}
