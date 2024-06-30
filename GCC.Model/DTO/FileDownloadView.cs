namespace GlobalCalendar.MVC.DTO
{
    public class FileDownloadView
    {
        public long UploadId { get; set; }
        public long TaskId { get; set; }
        public string TaskDocumnet { get; set; } = String.Empty;
        public string TaskFileName { get; set; } = String.Empty;
    }
}
