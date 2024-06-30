using System;
using System.ComponentModel.DataAnnotations;

namespace Chat.Web.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime? Timestamp { get; set; }
        public string FromUserName { get; set; }=string.Empty;
        public string FromFullName { get; set; }=string.Empty;
        [Required]
        public string Room { get; set; }=string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
