using Microsoft.AspNetCore.Http;

namespace GlobalCalendar.MVC.DTO
{
    public class TaskDetail
    {
        public int Id { get; set; }
        public int dtId { get; set; }
        public string TaskName { get; set; } = String.Empty;
        public string TaskDescription { get; set; } = String.Empty;
        public string Period { get; set; } = String.Empty;
        public string PrimaryOwner { get; set; } = String.Empty;
        public string SecondaryOwner { get; set; } = String.Empty;
        public string PrimaryApprover { get; set; } = String.Empty;
        public string SecondaryApprover { get; set; } = String.Empty;
        public string Comment { get; set; } = String.Empty;
        public string Comment2 { get; set; } = String.Empty;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string UploadFile { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int OverdueTask { get; set; }
        public int TaskDocId { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IFormFile UploadDoc { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public bool IsSubmitTask { get; set; }
        public DateTime Duedateforsubmission { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string DomainId { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
