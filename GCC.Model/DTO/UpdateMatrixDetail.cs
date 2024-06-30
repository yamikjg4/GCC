namespace GlobalCalendar.MVC.DTO
{
    public class UpdateMatrixDetail
    {
        public long TaskId { get; set; }
        public bool IsSubmitTask { get; set; }
        public string PrimayOwnerDomainId { get; set; }
        public string SecondaryOwnerDomainId { get; set; }
        public string PrimaryAproverDomainId { get; set; }
        public string SecondaryApproverDomainId { get; set; }

    }
}
