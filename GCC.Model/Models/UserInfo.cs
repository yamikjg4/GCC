using System.ComponentModel.DataAnnotations;

namespace GlobalCalendar.MVC.Models
{
    public class UserInfo
    {
        public string userId { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; } = String.Empty;
        public string EmailId { get; set; } = String.Empty;
        public string Roles { get; set; } = String.Empty;
        [Required(ErrorMessage = "DomainID Is Required")]
        public string DomainID { get; set; } = String.Empty;
        public string Designation { get; set; } = String.Empty;
        public string Manager { get; set; } = String.Empty;
        public string Mobile { get; set; } = String.Empty;
        public string Department { get; set; } = String.Empty;
        public string Grade { get; set; } = String.Empty;
        public string EmpCode { get; set; } = String.Empty;
        public string Telephone { get; set; } = String.Empty;
        public string location { get; set; } = String.Empty;
        public string Segment { get; set; } = String.Empty;
        public string Company { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public int ManagerCount { get; set; }
        public string Type { get; set; } = String.Empty;
        public string RoleId { get; set; } = String.Empty;
        public bool IsActive { get; set; }
    }
}
