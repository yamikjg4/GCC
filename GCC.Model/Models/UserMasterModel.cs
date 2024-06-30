using System.ComponentModel.DataAnnotations;

namespace GlobalCalendar.MVC.Models
{
    public class UserMasterModel
    {
        [Key]
        public int ID { get; set; }
        public string DomainId { get; set; } = string.Empty;
        public string Employee_Name { get; set; }= string.Empty;
        public string Segment { get; set; } = string.Empty;
        public string Email_ID { get; set; }=string.Empty;
        public bool IS_ACTIVE { get; set; }
        public int RoleId { get; set; }
    }
}
