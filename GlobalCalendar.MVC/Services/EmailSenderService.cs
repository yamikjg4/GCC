using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Utility;
using Hangfire;
using Microsoft.Extensions.Hosting;
namespace GlobalCalendar.MVC.Services
{
    public class EmailSenderService
    {

        private readonly CommonRepo _repo;
        /* private readonly ILogger<EmailSenderService> _logger;*/
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EmailSenderService(CommonRepo repo, IBackgroundJobClient backgroundJobClient)
        {
            _repo = repo;
            _backgroundJobClient = backgroundJobClient;
        }


       /* public async Task ExecuteAsync()
        {
            List<TaskTrigger> lstTrigger = new List<TaskTrigger>();
            var lst = _repo.GetTriggerList();

            foreach (var item in lst)
            {
                TaskTrigger task = new TaskTrigger();
                task.TemplateBody = item.TemplateBody;
                task.TemplateBody = task.TemplateBody.Replace("{User}", item.UserName);

                if (item.IsOverdue)
                {
                    task.TemplateBody = task.TemplateBody.Replace("{TaskType}", "Overdue tasks:");
                }
                else
                {
                    task.TemplateBody = task.TemplateBody.Replace("{TaskType}", "Approaching due date tasks:");
                }

                task.TemplateBody += "<tr>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.Company}</td>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.MonthYear}</td>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.TaskId}</td>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.TaskName}</td>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.Duedateforsubmission}</td>";
                task.TemplateBody += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{item.Duedateforapproval}</td>";
                task.TemplateBody += "</tr>";
                task.TemplateBody += "</table>";
                task.TemplateBody += "<br/><table style=\"border-collapse: collapse; width: 100%;\">\r\n";
                task.TemplateBody += "  <tr style=\"background-color: gray; font-weight: bold;\">\r\n";
                task.TemplateBody += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">Link</td>\r\n";
                task.TemplateBody += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">\r\n";
                task.TemplateBody += "      <a href=\"https://uat-int.suzlon.com/GlobalCalendar\">https://uat-int.suzlon.com/GlobalCalendar</a>\r\n";
                task.TemplateBody += "    </td>\r\n";
                task.TemplateBody += "  </tr>\r\n";
                task.TemplateBody += "</table>\r\n";
                task.TemplateBody += "<br/>\r\n<p>\r\n<strong>Regards,<br/>\r\nGlobal Close Controller\r\n</strong>\r\n</p>";
                task.TemplateBody += "<br/><span style=\"color: #777; display: block; text-align: center;\">Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox.</span>";

                // Send email
                _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "Notification for approaching due date and overdue tasks to the owner and approver", task.TemplateBody));

            }

            
        }
        public async Task EnqueueEmailJob(string to, string cc, string subject, string template)
        {
            await Utility.SMTPService.SendEmailCC(to, cc, subject, template);
        }*/


    }
}
