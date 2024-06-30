namespace GlobalCalendar.MVC.DTO
{
    public class BroadcastDTo
    {
        public int Type { get; set; }
        public string Message { get; set; }=string.Empty;
        public int Total { get; set; }
        public bool IsView { get; set; }
        public List<Microsoft.AspNetCore.Http.IFormFile> File { get; set; }
    }
    public class BroadCastTempDto
    {
        public List<BroadcastDTo> AllList { get; set; }
        public List<BroadcastDTo> UnreadList { get; set; }
    }
}
