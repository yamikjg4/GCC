namespace GlobalCalendar.MVC.Models
{
    public class BrodcastHistory
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int RoomId { get; set; }
        public string DomainId { get; set; }
        public bool IsRead { get; set; }
    }
}
