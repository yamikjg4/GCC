using Chat.Web.Data;
using GCC.Utility.Utility;
using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;

namespace GlobalCalendar.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CommonRepo _repo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ApplicationDbContext _context;
        private readonly TBSDAL _tbsDAL;
        public HomeController(ILogger<HomeController> logger, CommonRepo repo, IWebHostEnvironment webHostEnvironment, IBackgroundJobClient backgroundJobClient, ApplicationDbContext _context, TBSDAL tbsDAL)
        {
            _logger = logger;
            _repo = repo;
            _webHostEnvironment = webHostEnvironment;
            this._backgroundJobClient = backgroundJobClient;
            this._context = _context;
            _tbsDAL = tbsDAL;
        }
        public IActionResult Index()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (_userInfo.RoleId == "1")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    string DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
                    var lstownerchart = _repo.GetSubmitTaskChart(DomainId);
                    var lstApproverChart = _repo.GetApproveTaskChart(DomainId);
                   
                  
                    ViewBag.Chart = lstownerchart;
                    ViewBag.Chart2 = lstApproverChart;
                    return View();
                }
            }
            
           /* return View();*/
        }
     
        public JsonResult GetSession()
        {
            string DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
            
            return Json(new { data = DomainId });           
        }
        public JsonResult GetResult()
        {
            string DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
          
            var totalnotification = _context.Tbl_BrodcastMesHistrory.Where(x => x.DomainId == DomainId && x.IsRead==false).Count();
            return Json(new { total= totalnotification>0?totalnotification:0 });
        }
        public IActionResult NotificationMessage(int id)
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }

            _repo.UpdateViewStatus(id);
            return RedirectToAction("ViewAllNotification", "Home");
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ListOfPage()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string empcode = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            var lstTask = _repo.GetList(empcode);
#pragma warning restore CS8604 // Possible null reference argument.
            ViewBag.Id = empcode;
            return View(lstTask);
        }
        public IActionResult AcceptTasks(int Id)
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("ListOfPage", "Home");
        }
        public IActionResult SubmitTask(int Id)
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            //_webHostEnvironment.WebRootPath;
            var tasks = _repo.GetReason();
            ViewBag.Reason = new SelectList(tasks, "ReasonId", "ReasonName");
            var data = _repo.GetTaskDetailById(Id);
            data.Comment = String.Empty;
            /*return RedirectToAction("ListOfPage", "Home");*/
            return View(data);
        }
        public async Task EnqueueEmailJob(string to, string cc, string subject, string template)
        {
            await SMTPService.SendEmailCC(to, cc, subject, template);
        }
        [HttpPost]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> SubmitTask(TaskDetail obj, string btn)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                if (btn == "Save")
                {
                    TaskDetail taskDetail = new TaskDetail();
                    taskDetail.Id = obj.Id;
                    taskDetail.TaskDocId = obj.TaskDocId;
                    taskDetail.Comment = obj.Comment;
                    taskDetail.OverdueTask = obj.OverdueTask;
                    taskDetail.UploadFile = "";
#pragma warning disable CS8601 // Possible null reference assignment.
                    taskDetail.DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8601 // Possible null reference assignment.
                    _repo.SaveTaskDetail(taskDetail);
#pragma warning disable CS8601 // Possible null reference assignment.
                    TempPut objnew = new TempPut()
                    {
                        Id = obj.Id,
                        Date = DateTime.Now,
                        DomainId = Convert.ToString(HttpContext.Session.GetString("empCode")),
                        IsAccept = true
                    };
#pragma warning restore CS8601 // Possible null reference assignment.
                    _repo.UpdateStatus(objnew);
                    return RedirectToAction("ListOfPage", "Home");
                }
                else if (btn == "submit")
                {
                    EmailTemplateSender template = new EmailTemplateSender();
                    TaskDetail taskDetail = new TaskDetail();
                    taskDetail.Id = obj.Id;
                    taskDetail.TaskDocId = obj.TaskDocId;
                    taskDetail.Comment = obj.Comment;
                    taskDetail.OverdueTask = obj.OverdueTask;
#pragma warning disable CS8601 // Possible null reference assignment.
                    taskDetail.DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8601 // Possible null reference assignment.
                    taskDetail.UploadFile = "";
                    var lstdata = _repo.SaveTaskDetail(taskDetail);
#pragma warning disable CS8601 // Possible null reference assignment.
                    TempPut objnew = new TempPut()
                    {
                        Id = obj.Id,
                        Date = DateTime.Now,
                        DomainId = Convert.ToString(HttpContext.Session.GetString("empCode")),
                        IsAccept = true
                    };
#pragma warning restore CS8601 // Possible null reference assignment.
                    _repo.UpdateSubmit(objnew);
                    if (lstdata != null)
                    {
                        string UserEmail1 = string.Empty, UserEmail2 = string.Empty, AdminEmail1 = string.Empty, AdminEmail2 = string.Empty;
                        string[] nameArray = lstdata.UserName.Split(new string[] { "," }, StringSplitOptions.None);
                        string UserName = nameArray[0];
                        string[] AnameArray = lstdata.AdminUserName.Split(new string[] { "," }, StringSplitOptions.None);
                        string Approver = AnameArray[0];
                        string[] nameEmail = lstdata.EmailId.Split(new string[] { "," }, StringSplitOptions.None);
                        string[] AnameEmail = lstdata.AdminEmailId.Split(new string[] { "," }, StringSplitOptions.None);
                        UserEmail1 = nameEmail[0];
                        UserEmail2 = nameEmail[1];
                        AdminEmail1 = AnameEmail[0];
                        AdminEmail2 = AnameEmail[1];
                        template.Template1 = lstdata.Template1;
                        template.Template2 = lstdata.Template2;
                        template.Template1 = template.Template1.Replace("{User}", nameArray[0]);
                        template.Template2 = template.Template2.Replace("{User}", AnameArray[0]);
                        template.Template1 += "<tr>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.CompanyName}</td>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.MonthYear}</td>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.TaskId}</td>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.TaskName}</td>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.Duedateforsubmission}</td>";
                        template.Template1 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.Duedateforapproval}</td>";
                        template.Template1 += "</tr></table>";
                        template.Template1 += "<br/><table style=\"border-collapse: collapse; width: 100%;\">\r\n";
                        template.Template1 += "  <tr style=\"background-color: gray; font-weight: bold;\">\r\n";
                        template.Template1 += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">Link</td>\r\n";
                        template.Template1 += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">\r\n";
                        template.Template1 += "      <a href=\"https://uat-int.suzlon.com/GlobalCalendar\">https://uat-int.suzlon.com/GlobalCalendar</a>\r\n";
                        template.Template1 += "    </td>\r\n";
                        template.Template1 += "  </tr>\r\n";
                        template.Template1 += "</table>\r\n";
                        template.Template1 += "<br/>\r\n<p>\r\n<strong>Regards,<br/>\r\nGlobal Close Controller\r\n</strong>\r\n</p>";
                        template.Template1 += "<br/><span style=\"color: #777; display: block; text-align: center;\">Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox.</span>";


                        template.Template2 += "<tr>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.CompanyName}</td>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.MonthYear}</td>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.TaskId}</td>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.TaskName}</td>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.Duedateforsubmission}</td>";
                        template.Template2 += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{lstdata.Duedateforapproval}</td>";
                        template.Template2 += "</tr></table>";
                        template.Template2 += "<br/><table style=\"border-collapse: collapse; width: 100%;\">\r\n";
                        template.Template2 += "  <tr style=\"background-color: gray; font-weight: bold;\">\r\n";
                        template.Template2 += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">Link</td>\r\n";
                        template.Template2 += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">\r\n";
                        template.Template2 += "      <a href=\"https://uat-int.suzlon.com/GlobalCalendar\">https://uat-int.suzlon.com/GlobalCalendar</a>\r\n";
                        template.Template2 += "    </td>\r\n";
                        template.Template2 += "  </tr>\r\n";
                        template.Template2 += "</table>\r\n";
                        template.Template2 += "<br/>\r\n<p>\r\n<strong>Regards,<br/>\r\nGlobal Close Controller\r\n</strong>\r\n</p>";
                        template.Template2 += "<br/><span style=\"color: #777; display: block; text-align: center;\">Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox.</span>";

                        _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "Notification for tasks submitted for approval", template.Template1));
                        _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "Notification for new tasks assigned for approval", template.Template2));

                    }
                    return RedirectToAction("ListOfPage", "Home");
                }
                else
                {
                    return RedirectToAction("ListOfPage", "Home");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IActionResult ApproveTaskList()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string empcode = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
            var lstdet = _repo.GetApproveList(empcode);
#pragma warning restore CS8604 // Possible null reference argument.
            return View(lstdet);
        }
        public IActionResult ApproveView(int Id)
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var tasks = _repo.GetReason();
            ViewBag.Reason = new SelectList(tasks, "ReasonId", "ReasonName");
            var data = _repo.GETTaskDetailApprover(Id);
            data.Comment = string.Empty;
            /*return RedirectToAction("ListOfPage", "Home");*/
            return View(data);
        }
        [HttpPost]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> ApproveSubmitTask(TaskDetail obj, int TaskId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                TaskDetail taskDetail = new TaskDetail();
                taskDetail.Id = TaskId;
                taskDetail.TaskDocId = obj.TaskDocId;
                taskDetail.Comment = obj.Comment;
                taskDetail.OverdueTask = obj.OverdueTask;
                taskDetail.UploadFile = "";
#pragma warning disable CS8601 // Possible null reference assignment.
                taskDetail.DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8601 // Possible null reference assignment.
                _repo.SaveTaskDetail(taskDetail);
#pragma warning disable CS8601 // Possible null reference assignment.
                TempPut objnew = new TempPut()
                {
                    Id = TaskId,
                    Date = DateTime.Now,
                    DomainId = Convert.ToString(HttpContext.Session.GetString("empCode")),
                    IsAccept = true
                };
#pragma warning restore CS8601 // Possible null reference assignment.
                _repo.UpdateApprove(objnew);
                return RedirectToAction("ApproveTaskList", "Home");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult GetApprovedTaskList()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            List<GlobalCalendar.MVC.DTO.ListApproveTask> lstTask = new List<DTO.ListApproveTask>();
            if (!string.IsNullOrEmpty(DomainId))
            {
                var task= _repo.GetApprovedList(DomainId);
                if (task.Count > 0)
                {
                    lstTask = task;
                }
            }
            return View(lstTask);
        }
        public IActionResult ApprovedTaskList()
        {
            string DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
            List<GlobalCalendar.MVC.DTO.ListApproveTask> lstTask = new List<DTO.ListApproveTask>();
            if (!string.IsNullOrEmpty(DomainId))
            {
                var task = _repo.GetApprovedList(DomainId);
                if (task.Count > 0)
                {
                    lstTask = task;
                }
            }
            if (lstTask.Count > 0) {
              
                return Json(new {data=lstTask });
            }
            else
            {
                return Json(new { data=""});
            }
           
        }
        [HttpPost]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> QueryOwner(string Comment, int dtId, int TaskDocId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                TaskDetail objTask = new TaskDetail();
                objTask.Id = dtId;
                objTask.Comment = Comment;
                objTask.TaskDocId = TaskDocId;
#pragma warning disable CS8601 // Possible null reference assignment.
                objTask.DomainId = Convert.ToString(HttpContext.Session.GetString("empCode"));
#pragma warning restore CS8601 // Possible null reference assignment.
                var EmailDetails = _repo.RejectApprover(objTask);
                string Comment2 = "";
                Comment2 = "Task Name : " + EmailDetails.TaskName + "<br/>" + "Feedback : " + objTask.Comment;
                _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "REJECT APPROVER DURING REASON", Comment2));
                /*              await Utility.SMTPService.SendEmail("Baishali.Biswas@suzlon.com,Yamik.Mindnerves@suzlon.com", "REJECT APPROVER DURING REASON", Comment2);*/
                return Json(new { data = "Sucess" });
            }
            catch (Exception ex)
            {
                return Json(new { data = $"{ex.Message}" });
            }
        }
        public async Task<IActionResult> Upload(int param1, List<IFormFile> files)
        {
            List<FileDownloadView> lstdata = new List<FileDownloadView>();
            foreach (var file in files)
            {
                DTO.UploadFile upload = new DTO.UploadFile();
                upload.TaskId = param1;
                upload.TaskFileName = file.FileName;
                string ExsistsFile = _repo.CheckFileExsists(upload);
                if (!string.IsNullOrEmpty(ExsistsFile))
                {
                    await DocumentHelper.DeleteFileasync(ExsistsFile);
                }
                upload.TaskDocumnet = await DocumentHelper.UploadDocumentAsync(file);
                lstdata = _repo.GetFileList(upload);
            }


            if (lstdata.Count > 0)
            {
                return Json(new { data = lstdata });
            }
            return Json(new { data = "" });
        }

        public IActionResult DeleteDoc(int param1, int param2)
        {
            try
            {
                bool issucess = _repo.DeleteRecord(param1);
                if (issucess)
                {
                    var lst = _repo.GetUploadFileList(param2); // Assuming param2 is an integer
                    if (lst.Count > 0)
                    {
                        return Json(new { data = lst });
                    }
                    else
                    {
                        return Json(new { data = "" });
                    }
                }
                else
                {
                    return Json(new { data = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IActionResult GetDocumentDetailByTask(int id)
        {
            var lst = _repo.GetUploadFileList(id);

            if (lst.Count > 0)
            {
                return Json(new { data = lst });
            }
            return Json(new { data = "" });
        }
        public IActionResult GetAllComment(int id)
        {
            var lst = _repo.GetAllComment(id);
            if (lst.Count > 0)
            {
                return Json(new { data = lst });
            }
            return Json(new { data = "" });
        }

        public IActionResult Download(string fileName)
        {
            // Construct the full path to the file
            string filePath = Path.Combine(@"\\SELPUNNETSCH01\Suzlon_Inhouse_File_Server_UAT\GCC\", "uploads", fileName);

            if (System.IO.File.Exists(filePath))
            {
                // Return the file for download
                return PhysicalFile(filePath, "application/octet-stream", fileName);
            }
            else
            {
                // Return a 404 Not Found response
                return NotFound();
            }
        }

    }
}