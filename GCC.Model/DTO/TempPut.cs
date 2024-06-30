namespace GlobalCalendar.MVC.DTO
{
    public class TempPut
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public bool IsAccept { get; set; }
    }
}
