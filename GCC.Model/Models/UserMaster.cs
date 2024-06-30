using System.ComponentModel.DataAnnotations;

namespace GlobalCalendar.MVC.Models
{
    public class UserMaster
    {
        public long userId { get; set; }
        public string DomainID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Segment { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}
