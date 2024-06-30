using GCC.Utility.Utility;
using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace GlobalCalendar.MVC.Controllers
{
    public class AdminReportController : Controller
    {
        private readonly CommonRepo _repo;
        public AdminReportController(CommonRepo repo)
        {
            _repo = repo;
        }

        public IActionResult CompanyWise()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Month> months = new List<Month>
        {
            new Month(1, "January"),
            new Month(2, "February"),
            new Month(3, "March"),
            new Month(4, "April"),
            new Month(5, "May"),
            new Month(6, "June"),
            new Month(7, "July"),
            new Month(8, "August"),
            new Month(9, "September"),
            new Month(10, "October"),
            new Month(11, "November"),
            new Month(12, "December")
        };
            ViewBag.Months = new SelectList(months, "Id", "Name");
            var lstyear = _repo.GetListOfYear();
            ViewBag.YearMaster = new SelectList(lstyear, "YEAR", "YEAR");
            var lstdata = _repo.GetAllCompanyList();
            CompanyMaster obj = new CompanyMaster();
            obj.CompanyId = 687;
            obj.CompanyName = "All";
            lstdata.Add(obj);
            ViewBag.Company = new SelectList(lstdata, "CompanyId", "CompanyName");
            return View();
        }
        public IActionResult CreateReport(TempDto objdata)
        {
            if (objdata.rept == 1)
            {
                var lst = _repo.GetCompanyWiseReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                return Json(new { reptname = "CompanyWiseRept", data = lst });
            }
            else if (objdata.rept == 2)
            {
                var lstdata = _repo.GetOwnerWiseReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                return Json(new { reptname = "OwnerWiseRept", data = lstdata });
            }
            else if (objdata.rept == 3)
            {
                var lstdata = _repo.GetApproveList(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                return Json(new { reptname = "ApproverWiseRept", data = lstdata });
            }
            else
            {
                return Json(new { reptname = "", data = "" });
            }
        }
        public IActionResult CreateErrorReport(TempDto objdata)
        {
            var lstdata = _repo.GetErrorReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);

            return Json(new { data = lstdata });


        }
        public IActionResult ExportToExcel(TempDto objdata)
        {
            DataTable dataTable;
            switch (objdata.rept)
            {
                case 1:
                    var lst = _repo.GetCompanyWiseReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                    dataTable = CommonMethod.ConvertToDataTable(lst);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["Companyname"].ColumnName = "Company code/Company name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTask"].ColumnName = "Total tasks";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskCompleted"].ColumnName = "Total task complete";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingNotdue"].ColumnName = "Total tasks pending not due";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingOverdue"].ColumnName = "Total tasks pending overdue";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    break;
                case 2:
                    var lstdata = _repo.GetOwnerWiseReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                    dataTable = CommonMethod.ConvertToDataTable(lstdata);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["EmployeeName"].ColumnName = "Task owner name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["CompanyName"].ColumnName = "Company code/Company name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTask"].ColumnName = "Total tasks";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["IsApprove"].ColumnName = "Total task complete";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingNotDue"].ColumnName = "Total tasks pending not due";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingOverDue"].ColumnName = "Total tasks pending overdue";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    break;
                case 3:
                    var lstdatas = _repo.GetApproveList(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
                    dataTable = CommonMethod.ConvertToDataTable(lstdatas);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["EmployeeName"].ColumnName = "Task approver name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["CompanyName"].ColumnName = "Company code/Company name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTask"].ColumnName = "Total tasks";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["IsApprove"].ColumnName = "Total task complete";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingNotDue"].ColumnName = "Total tasks pending not due";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    dataTable.Columns["TotalTaskPendingOverDue"].ColumnName = "Total tasks pending overdue";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    break;
                default:
                    return BadRequest("Invalid report type.");
            }

            var stream = CommonMethod.ExportToExcel(dataTable);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");

        }
        public IActionResult ExportErrorReport(TempDto objdata)
        {
            DataTable dataTable;
            var lst = _repo.GetErrorReport(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
            dataTable = CommonMethod.ConvertToDataTable(lst);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dataTable.Columns["Company"].ColumnName = "Company code Company name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dataTable.Columns["TaskNo"].ColumnName = "Task number";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dataTable.Columns["TaskName"].ColumnName = "Task name";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dataTable.Columns["EmpName"].ColumnName = "Task owner";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            dataTable.Columns["Comment"].ColumnName = "Comments for rejection";
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var stream = CommonMethod.ExportToExcel(dataTable);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");
        }
        public IActionResult ErrorReport()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Month> months = new List<Month>
        {
            new Month(1, "January"),
            new Month(2, "February"),
            new Month(3, "March"),
            new Month(4, "April"),
            new Month(5, "May"),
            new Month(6, "June"),
            new Month(7, "July"),
            new Month(8, "August"),
            new Month(9, "September"),
            new Month(10, "October"),
            new Month(11, "November"),
            new Month(12, "December")
        };
            ViewBag.Months = new SelectList(months, "Id", "Name");
            var lstyear = _repo.GetListOfYear();
            ViewBag.YearMaster = new SelectList(lstyear, "YEAR", "YEAR");
            var lstdata = _repo.GetAllCompanyList();
            CompanyMaster obj = new CompanyMaster();
            obj.CompanyId = 687;
            obj.CompanyName = "All";
            lstdata.Add(obj);
            ViewBag.Company = new SelectList(lstdata, "CompanyId", "CompanyName");
            return View();
        }
        public IActionResult ResourceReport()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Month> months = new List<Month>
        {
            new Month(1, "January"),
            new Month(2, "February"),
            new Month(3, "March"),
            new Month(4, "April"),
            new Month(5, "May"),
            new Month(6, "June"),
            new Month(7, "July"),
            new Month(8, "August"),
            new Month(9, "September"),
            new Month(10, "October"),
            new Month(11, "November"),
            new Month(12, "December")
        };
            ViewBag.Months = new SelectList(months, "Id", "Name");
            var lstyear = _repo.GetListOfYear();
            ViewBag.YearMaster = new SelectList(lstyear, "YEAR", "YEAR");
            var lstdata = _repo.GetAllCompanyList();
            CompanyMaster obj = new CompanyMaster();
            obj.CompanyId = 687;
            obj.CompanyName = "All";
            lstdata.Add(obj);
            ViewBag.Company = new SelectList(lstdata, "CompanyId", "CompanyName");
            return View();
        }
        public IActionResult ExceptionReport()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Month> months = new List<Month>
        {
            new Month(1, "January"),
            new Month(2, "February"),
            new Month(3, "March"),
            new Month(4, "April"),
            new Month(5, "May"),
            new Month(6, "June"),
            new Month(7, "July"),
            new Month(8, "August"),
            new Month(9, "September"),
            new Month(10, "October"),
            new Month(11, "November"),
            new Month(12, "December")
        };
            ViewBag.Months = new SelectList(months, "Id", "Name");
            var lstyear = _repo.GetListOfYear();
            ViewBag.YearMaster = new SelectList(lstyear, "YEAR", "YEAR");
            var lstdata = _repo.GetAllCompanyList();
            CompanyMaster obj = new CompanyMaster();
            obj.CompanyId = 687;
            obj.CompanyName = "All";
            lstdata.Add(obj);
            ViewBag.Company = new SelectList(lstdata, "CompanyId", "CompanyName");
            return View();
        }
        public IActionResult CreateExceptionReport(TempDto objData)
        {
            var lstdata = _repo.GetExceptionList(Convert.ToInt32(objData.Year), Convert.ToInt32(objData.company), objData.Month);
            return Json(new { data = lstdata });
        }
        public IActionResult CreateResourceReport(TempDto objdata)
        {
            var lstdata = _repo.GetTaskResourceRepts(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);

            return Json(new { data = lstdata });
        }
        public IActionResult ExportExceptionReport(TempDto objdata)
        {
            DataTable dataTable;
            var lstdata = _repo.GetExceptionList(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
            dataTable = CommonMethod.ConvertToDataTable(lstdata);
            dataTable.Columns["Company"].ColumnName = "Company code/ Company name";
            dataTable.Columns["OwnerName"].ColumnName = "Task owner";
            dataTable.Columns["ApproverName"].ColumnName = "Task approver";
            dataTable.Columns["SubmissionDueDate"].ColumnName = "Submit Delay Of Day";
            dataTable.Columns["SubmissionApprovalDate"].ColumnName = "Approve Delay Of Day";
            dataTable.Columns["ReasonName"].ColumnName = "Reason for delay";
            var stream = CommonMethod.ExportToExcel(dataTable);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");

        }
        public IActionResult ExportResourceReport(TempDto objdata)
        {
            DataTable dataTable;
            var lstdata = _repo.GetTaskResourceRepts(Convert.ToInt32(objdata.Year), Convert.ToInt32(objdata.company), objdata.Month);
            dataTable = CommonMethod.ConvertToDataTable(lstdata);
            dataTable.Columns["EmpName"].ColumnName = "Task owner name";
            dataTable.Columns["Company"].ColumnName = "Company code/ Company name";
            dataTable.Columns["Period"].ColumnName = "Period";
            dataTable.Columns["TotalTask"].ColumnName = "Total tasks assigned";
            dataTable.Columns["Percentage"].ColumnName = "Percentage of total tasks";
            var stream = CommonMethod.ExportToExcel(dataTable);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExportedData.xlsx");
        }
        public IActionResult OwnerReportDetail(string employee, long year, string company, int Month)
        {

            string input = company;

            // Find the position of the '/' character
            int splitIndex = input.IndexOf('/');

            // Extract the numerical part before the '/'
            string numericalPart = input.Substring(0, splitIndex);

            // Parse the numerical part into an integer variable
            int number;
            number = Convert.ToInt32(numericalPart);
            var lstdata = _repo.GetOwnerWiseReport(Convert.ToInt32(year), number, Month);
            OwnerTaskSummary objdata = new OwnerTaskSummary();
            if (lstdata != null)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                objdata = lstdata.FirstOrDefault(x => x.EmployeeName == employee);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            OwnerWiseDetail owner = new OwnerWiseDetail();
#pragma warning disable CS8601 // Possible null reference assignment.
            owner.Owner = objdata;
#pragma warning restore CS8601 // Possible null reference assignment.
            owner.lstownerwiselst = _repo.GetOwnerWiseReport(Convert.ToInt32(year), number, Month, employee);
            return View(owner); // Assuming you have a corresponding view for this action
        }
        public IActionResult ApproverReportDetail(string employee, long year, string company, int Month)
        {
            string input = company;

            // Find the position of the '/' character
            int splitIndex = input.IndexOf('/');

            // Extract the numerical part before the '/'
            string numericalPart = input.Substring(0, splitIndex);

            // Parse the numerical part into an integer variable
            int number;
            number = Convert.ToInt32(numericalPart);
            var lstdata = _repo.GetApproveList(Convert.ToInt32(year), number, Month);
            ApproverTaskSummary objdata = new ApproverTaskSummary();
            if (lstdata != null)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                objdata = lstdata.FirstOrDefault(x => x.EmployeeName == employee);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            OwnerWiseDetail owner = new OwnerWiseDetail();
#pragma warning disable CS8601 // Possible null reference assignment.
            owner.Approver = objdata;
#pragma warning restore CS8601 // Possible null reference assignment.
            owner.lstownerwiselst = _repo.GetApproveWiseReport(Convert.ToInt32(year), number, Month, employee);
            return View(owner); // Assuming you have a corresponding view for this action
        }
    }

}
