using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GlobalCalendar.MVC.Models
{
    public class CompanyWiseDate
    {
        public int CompanyId { get; set; }
        [DataType(DataType.Date)]
        public DateTime CloseDate { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IFormFile UploadFile { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
