namespace GlobalCalendar.MVC.DTO
{
    public class MatreixModel
    {
        public long TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Duedateforsubmission { get; set; }
        public string Duedateforapproval { get; set; }
        public string PrimaryOwnerName { get; set; }
        public string SecondaryOwnerName { get; set; }
        public string PrimaryApproverName { get; set; }
        public string SecondaryApproverName { get; set; }

    }
}
