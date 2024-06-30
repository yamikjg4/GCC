using GCC.Utility.Utility;
using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Data;

namespace GlobalCalendar.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly CommonRepo _repo;
        private readonly IBackgroundJobClient _backgroundJobClient;
      private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(CommonRepo repo, IBackgroundJobClient backgroundJobClient, IWebHostEnvironment webHostEnvironment)
        {
            _repo = repo;
            _backgroundJobClient = backgroundJobClient;
            _webHostEnvironment = webHostEnvironment;
        }

        private DataTable ReadExcelToDataTable(Stream fileStream, string Column2)
        {
            DataTable dataTable = new DataTable();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int row = 6; row < 7; row++)
                {
                    // Assuming the headers are in the first row
                    for (int col = 3; col <= 28; col++)
                    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        string columnName = worksheet.Cells[row, col].Value?.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                        dataTable.Columns.Add(columnName);
                        /* if (col == 28)
                         {

                             columnName = "Year";
                             dataTable.Columns.Add(columnName);
                             columnName = "Month";
                             dataTable.Columns.Add(columnName);
                         }*/
                    }
                }
                /* dataTable.Columns.Add("UserType",typeof(string));*/

                for (int row = 7; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dataRow = dataTable.NewRow();

                    for (int col = 3; col <= 28; col++)
                    {
                        object cellValue = worksheet.Cells[row, col].Value;
                        dataRow[col - 3] = cellValue != null ? cellValue.ToString() : null;
                        /* if (col == 28)
                         {

                             dataRow[col - 3 + 1] = DateTime.Now.Year;
                             dataRow[col - 3 + 2] = DateTime.Now.Month;
                         }*/
                    }

                    dataTable.Rows.Add(dataRow);
                }

            }

            return dataTable;
        }
 
        public IActionResult Index()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
           
            return View();
        }
        public JsonResult GetRes(string Type)
        {
            if(Type == "M")
            {
                var lst = _repo.GetChart();
                var data = new Dictionary<string, int>();
                foreach (var item in lst)
                {

                    data.Add(item.CompanyName, item.Total);
                }
                /* data.Add("SGWPL", 200);
                 data.Add("SGL", 200);*/
                List<TaskSummaryViewModel> lstTaskSummary = new List<TaskSummaryViewModel>();
                var lsttask = _repo.GetCompanyWiseReport(Convert.ToInt32(DateTime.Now.Year), 687, 0);
                foreach (var item in lsttask)
                {
                    TaskSummaryViewModel model = new TaskSummaryViewModel();
                    model = item;
                    if (model.Companyname == "1000/SEL")
                    {
                        model.Companyname = "SEL";
                    }
                    else if (model.Companyname == "1700/SGWPL")
                    {
                        model.Companyname = "SGWPL";
                    }
                    else if (model.Companyname == "2200/SGSL")
                    {
                        model.Companyname = "SGSL";
                    }
                    else if (model.Companyname == "2900/SE Forge")
                    {
                        model.Companyname = "SE Forge";
                    }
                    lstTaskSummary.Add(model);
                }
               return Json(new {data=data , lstlstTaskSummary=lstTaskSummary });
            }
            else
            {
                var lst = _repo.GetChart();
                var data = new Dictionary<string, int>();
                foreach (var item in lst)
                {

                    data.Add(item.CompanyName, item.Total);
                }
                /* data.Add("SGWPL", 200);
                 data.Add("SGL", 200);*/
                List<TaskSummaryViewModel> lstTaskSummary = new List<TaskSummaryViewModel>();
                var lsttask = _repo.GetCompanyWiseReport(Convert.ToInt32(DateTime.Now.Year), 687, 0);
                foreach (var item in lsttask)
                {
                    TaskSummaryViewModel model = new TaskSummaryViewModel();
                    model = item;
                    if (model.Companyname == "1000/SEL")
                    {
                        model.Companyname = "SEL";
                    }
                    else if (model.Companyname == "1700/SGWPL")
                    {
                        model.Companyname = "SGWPL";
                    }
                    else if (model.Companyname == "2200/SGSL")
                    {
                        model.Companyname = "SGSL";
                    }
                    else if (model.Companyname == "2900/SE Forge")
                    {
                        model.Companyname = "SE Forge";
                    }
                    lstTaskSummary.Add(model);
                }
                return Json(new { data = data, lstlstTaskSummary = lstTaskSummary });
            }
        }
        public IActionResult UploadExcel()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public async Task EnqueueEmailJob(string to, string cc, string subject, string template)
        {
            await SMTPService.SendEmailCC(to, cc, subject, template);
        }
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a file." });
                }

                if (!CommonMethod.CheckExcelContent(file))
                {
                    return Json(new { success = false, message = "Please select a valid Excel file." });
                }
                
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using (var package = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        // Specify the range you want to read (e.g., "B2:B4")
                        var startRow = 2;
                        var endRow = 4;
                        var dataRange = worksheet.Cells[$"B{startRow}:B{endRow}"];

                        // Get the values from the specified range
                        var rowData = dataRange.Select(cell => cell.Text).ToList();

                        if (rowData.Count < 3)
                        {
                            return Json(new { success = false, message = "Insufficient data in the specified range." });
                        }

                        var objDataModel = new DataModel
                        {
                            Column1 = rowData[0],
                            Column2 = (rowData[1]?.Replace("Entity:", "") ?? "").Trim(),
                            Column3 = rowData[2]
                        };

                        var data = _repo.GetDepartmentDetail(objDataModel.Column2);

                        DataTable dt = ReadExcelToDataTable(file.OpenReadStream(), objDataModel.Column2);

                        var res = _repo.PostDepartmentDetail(dt);

                        if (res.ErrorMessage == "Data Inserted Successfully" && string.IsNullOrEmpty(res.ErrorProcedure))
                        {
                            string filename=DocumentHelper.UploadExcelAsync(file);
                            if (filename != null)
                            {
                                string dfilename = filename;
                                string fname = file.FileName;
                                _repo.UploadHistoryExcel(dfilename,fname);
                            }
                            return Json(new { success = true, message = "File uploaded successfully" });
                           
                        }
                        else
                        {
                            return Json(new { success = false, message = res.ErrorMessage + " , " + res.ErrorProcedure });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error uploading file: {ex.Message}" });
            }
        }


        public IActionResult AddClosingCalender()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(_repo.GetCompanyClosingCalList());
        }
        
        public IActionResult AddClosingClaenderDetail()
        {

            var companies = _repo.GetCompanyList();
            DTOCompanyList companyList = new DTOCompanyList();
            companyList.CompanyId = 687;
            companyList.CompanyName = "All Company";
            companies.Add(companyList);
            ViewBag.CompanyList = new SelectList(companies, "CompanyId", "CompanyName");
            return View();
        }
        public PartialViewResult OpenPartialview(int TaskId)
        {
            var res = _repo.GetMatrixDeatil(TaskId);
            var res1 = _repo.GetUser();
            ViewBag.UserList = new SelectList(res1, "DomainId", "EmpName");
            return PartialView(res); 
        }
     
        [HttpPost]
        public IActionResult PostPartialData(UpdateMatrixDetail Model)
        {
            var lst = _repo.UpdateMatrix(Model);
            if (lst.Count > 0)
            {
                /*var distinctEmailIds = lst.Select(x => x.EmailId).Distinct();
                foreach (var emailId in distinctEmailIds)
                {
                    string Email1 = string.Empty;
                    string Email2 = string.Empty;
                    int cnt = 0;
                    var DTOSendingEmail = new EmailSendingDto();
                    var itemsWithDistinctEmail = lst.Where(x => x.EmailId == emailId);
                    foreach (var group in itemsWithDistinctEmail)
                    {
                        if (!string.IsNullOrEmpty(group.EmailId) && !string.IsNullOrEmpty(group.UserName))
                        {
                            if (cnt == 0)
                            {
                                DTOSendingEmail.Template = group.Template;
                                string names = group.UserName;
                                string email = group.EmailId;

                                // Split the string by comma and space
                                string[] nameArray = names.Split(new string[] { "," }, StringSplitOptions.None);
                                string[] EmailArray = email.Split(new string[] { "," }, StringSplitOptions.None);
                                // Get the first value
                                Email1 = EmailArray[0];
                                Email2 = EmailArray[1];
                                string firstValue = nameArray[0];
                                DTOSendingEmail.Template = DTOSendingEmail.Template.Replace("{User}", firstValue);
                                cnt++;
                            }
                            DTOSendingEmail.Template += "<tr>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.CompanyName}</td>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.MonthYear}</td>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.TaskId}</td>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.TaskName}</td>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.Duedateforsubmission}</td>";
                            DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.Duedateforapproval}</td>";
                            DTOSendingEmail.Template += "</tr>";
                            // Access properties of each item

                            // Access other properties as needed
                        }


                    }
                    DTOSendingEmail.Template += "</table>";
                    DTOSendingEmail.Template += "<br/><table style=\"border-collapse: collapse; width: 100%;\">\r\n";
                    DTOSendingEmail.Template += "  <tr style=\"background-color: gray; font-weight: bold;\">\r\n";
                    DTOSendingEmail.Template += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">Link</td>\r\n";
                    DTOSendingEmail.Template += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">\r\n";
                    DTOSendingEmail.Template += "      <a href=\"https://uat-int.suzlon.com/GlobalCalendar\">https://uat-int.suzlon.com/GlobalCalendar</a>\r\n";
                    DTOSendingEmail.Template += "    </td>\r\n";
                    DTOSendingEmail.Template += "  </tr>\r\n";
                    DTOSendingEmail.Template += "</table>\r\n";
                    DTOSendingEmail.Template += "<br/>\r\n<p>\r\n<strong>Regards,<br/>\r\nGlobal Close Controller\r\n</strong>\r\n</p>";
                    DTOSendingEmail.Template += "<br/><span style=\"color: #777; display: block; text-align: center;\">Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox.</span>";


                    _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "Notification for new tasks assigned for submission", DTOSendingEmail.Template));
                }*/
                return RedirectToAction("GetMatrixList");
            }

            return RedirectToAction("GetMatrixList");
        }
        [HttpPost]
        public IActionResult AddClosingClaenderDetail(CompanyWiseDate model)
        {
            try
            {

                var lst = _repo.PostCompnayWithTime(model);
                if (lst.Count > 0)
                {
                    var distinctEmailIds = lst.Select(x => x.EmailId).Distinct();
                    foreach (var emailId in distinctEmailIds)
                    {
                        string Email1 = string.Empty;
                        string Email2 = string.Empty;
                        int cnt = 0;
                        var DTOSendingEmail = new EmailSendingDto();
                        var itemsWithDistinctEmail = lst.Where(x => x.EmailId == emailId);
                        foreach (var group in itemsWithDistinctEmail)
                        {
                            if (!string.IsNullOrEmpty(group.EmailId) && !string.IsNullOrEmpty(group.UserName))
                            {
                                if (cnt == 0)
                                {
                                    DTOSendingEmail.Template = group.Template;
                                    string names = group.UserName;
                                    string email = group.EmailId;

                                    // Split the string by comma and space
                                    string[] nameArray = names.Split(new string[] { "," }, StringSplitOptions.None);
                                    string[] EmailArray = email.Split(new string[] { "," }, StringSplitOptions.None);
                                    // Get the first value
                                    Email1 = EmailArray[0];
                                    Email2 = EmailArray[1];
                                    string firstValue = nameArray[0];
                                    DTOSendingEmail.Template = DTOSendingEmail.Template.Replace("{User}", firstValue);
                                    cnt++;
                                }
                                DTOSendingEmail.Template += "<tr>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.CompanyName}</td>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.MonthYear}</td>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.TaskId}</td>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.TaskName}</td>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.Duedateforsubmission}</td>";
                                DTOSendingEmail.Template += $"<td style=\"border: 1px solid black; padding: 8px; text-align: left;\">{group.Duedateforapproval}</td>";
                                DTOSendingEmail.Template += "</tr>";
                                // Access properties of each item

                                // Access other properties as needed
                            }


                        }
                        DTOSendingEmail.Template += "</table>";
                        DTOSendingEmail.Template += "<br/><table style=\"border-collapse: collapse; width: 100%;\">\r\n";
                        DTOSendingEmail.Template += "  <tr style=\"background-color: gray; font-weight: bold;\">\r\n";
                        DTOSendingEmail.Template += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">Link</td>\r\n";
                        DTOSendingEmail.Template += "    <td style=\"border: 0.5px solid black; padding: 8px; text-align: center;\">\r\n";
                        DTOSendingEmail.Template += "      <a href=\"https://uat-int.suzlon.com/GlobalCalendar\">https://uat-int.suzlon.com/GlobalCalendar</a>\r\n";
                        DTOSendingEmail.Template += "    </td>\r\n";
                        DTOSendingEmail.Template += "  </tr>\r\n";
                        DTOSendingEmail.Template += "</table>\r\n";
                        DTOSendingEmail.Template += "<br/>\r\n<p>\r\n<strong>Regards,<br/>\r\nGlobal Close Controller\r\n</strong>\r\n</p>";
                        DTOSendingEmail.Template += "<br/><span style=\"color: #777; display: block; text-align: center;\">Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox.</span>";


                        _backgroundJobClient.Enqueue(() => EnqueueEmailJob("Yamik.Mindnerves@suzlon.com", "Baishali.Biswas@suzlon.com", "Notification for new tasks assigned for submission", DTOSendingEmail.Template));
                    }
                }

                return RedirectToAction("AddClosingCalender", "Admin");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddBroadcastMessage()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddBroadcastMessage(BroadcastDTo model)
        {
           var ret= _repo.PostBroadCastMes(model);
            if (ret != 0)
            {
                foreach (var item in model.File) {
                    string FileName=await DocumentHelper.UploadDocumentAsync(item);
                    _repo.UploadFile(FileName,ret);
                }
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("AddBroadcastMessage","Admin");
            }
            
           
             
        }
        public IActionResult GetMatrixList()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var res= _repo.GetMatrixList();
            return View(res);
        }
    }
}
