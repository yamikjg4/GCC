using System.ComponentModel;

namespace GlobalCalendar.MVC.DTO
{
    public class ApproverTaskSummary
    {
        [DisplayName("Task approver name")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string EmployeeName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /* public string DomainId { get; set; }*/
        [DisplayName("Company code Company name")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string CompanyName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [DisplayName("TotalTask")]
        public int TotalTask { get; set; }
        [DisplayName("Total task complete")]
        public int IsApprove { get; set; }
        [DisplayName("Total tasks pending not due")]
        public int TotalTaskPendingNotDue { get; set; }
        [DisplayName("Total tasks pending overdue")]
        public int TotalTaskPendingOverDue { get; set; }
    }
}
