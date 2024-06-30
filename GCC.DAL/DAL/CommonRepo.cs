using Chat.Web.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using GlobalCalendar.DAL;
using GlobalCalendar.MVC.DTO;
using GlobalCalendar.MVC.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;

namespace GlobalCalendar.MVC.DAL
{
    public class CommonRepo
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        GlobalCalendar.DAL.DBConnection objDb = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /*     SAPDBConnection objSAPDb = null;*/
        public CommonRepo()
        {
            objDb = new DBConnection();
            /*  objSAPDb = new SAPDBConnection();*/
        }
        public List<string> GetRoleWiseDomain(int RoleId)
        {
            List<string> lst = new List<string>();
            List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@RoleId", SqlDbType=SqlDbType.Int, Value=RoleId }
            };
            var dt = objDb.ExecuteReader("[SP_Rolewise]", lstparam);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    if (dr != null)
                    {
                        string DomainId = Convert.ToString(dr["DomainId"]);
                        lst.Add(DomainId);
                    }
               }
            }
            return lst;
          }
        public DepartmentModel GetDepartmentDetail(string Department)
        {
            List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@DeptName", SqlDbType=SqlDbType.NVarChar, Value=Department }
            };
            var Dept = new DepartmentModel();
            var dt = objDb.ExecuteReader("[sp_departmentwisedate]", lstparam);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    if (dr != null)
                    {

#pragma warning disable CS8601 // Possible null reference assignment.
                        Dept.DepartMent = Convert.ToString(dr["DepartMent"]);
#pragma warning restore CS8601 // Possible null reference assignment.
                        Dept.StartDate = Convert.ToInt32(dr["StartDate"]);
                    }
                }

            }
            return Dept;
        }
        public Response PostDepartmentDetail(DataTable Dt)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@tbltask", SqlDbType=SqlDbType.Structured, Value=Dt },
             /* new SqlParameter { ParameterName="@CompanyId", SqlDbType=SqlDbType.BigInt, Value=model.CompanyId },
              new SqlParameter{ ParameterName ="@ClosingDate",SqlDbType=SqlDbType.DateTime,Value=model.CloseDate},*/
         /*     new SqlParameter{ ParameterName ="@size",SqlDbType=SqlDbType.Int,Value=size}*/
            };
                Response objResponse = new Response();
                var dt = objDb.ExecuteReader("[sp_inserttask]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    objResponse.ErrorMessage = dr.Table.Columns.Contains("ErrorMessage") ? Convert.ToString(dr["ErrorMessage"].ToString()) : String.Empty;
                    objResponse.ErrorProcedure = dr.Table.Columns.Contains("ErrorProcedure") ? Convert.ToString(dr["ErrorProcedure"].ToString()) : String.Empty;
                }
                /*if (dt.Rows.Count > 0)
                {
                   for(int i=0;i< dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        EmailSendingDto obj = new EmailSendingDto();
                        obj.TaskId = dr.Table.Columns.Contains("TaskId") ? Convert.ToInt32(dr["TaskId"].ToString()) : 0;
                        obj.CompanyName = dr.Table.Columns.Contains("CompanyName") ? Convert.ToString(dr["CompanyName"].ToString()) : String.Empty;
                        obj.EmailId = dr.Table.Columns.Contains("EmailId") ? Convert.ToString(dr["EmailId"].ToString()) : String.Empty;
                        obj.UserName = dr.Table.Columns.Contains("UserName") ? Convert.ToString(dr["UserName"].ToString()) : String.Empty;
                        obj.MonthYear = dr.Table.Columns.Contains("MonthYear") ? Convert.ToString(dr["MonthYear"].ToString()) : String.Empty;
                        obj.Duedateforsubmission = dr.Table.Columns.Contains("Duedateforsubmission") ? Convert.ToString(dr["Duedateforsubmission"].ToString()) : String.Empty;
                        obj.Duedateforapproval = dr.Table.Columns.Contains("Duedateforapproval") ? Convert.ToString(dr["Duedateforapproval"].ToString()) : String.Empty;
                        obj.TaskName = dr.Table.Columns.Contains("TaskName") ? Convert.ToString(dr["TaskName"].ToString()) : String.Empty;
                        obj.Template = dr.Table.Columns.Contains("Template") ? Convert.ToString(dr["Template"].ToString()) : String.Empty;
                        
                        lst.Add(obj);
                    }
                }*/
                return objResponse;
            }
            catch (Exception ex)
            {
                /*  throw ex;*/
                throw ex;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public List<Chart> GetChart()
        {

            List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@Year", SqlDbType=SqlDbType.Int, Value=DateTime.Now.Year },
              new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=DateTime.Now.Month },

            };
            List<Chart> lstchart = new List<Chart>();
            var dt = objDb.ExecuteReader("[sp_Chart]", lstparam);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    Chart objchart = new Chart();
#pragma warning disable CS8601 // Possible null reference assignment.
                    objchart.CompanyName = dr.Table.Columns.Contains("CompanyName") ? Convert.ToString(dr["CompanyName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
                    objchart.Total = dr.Table.Columns.Contains("Total") ? Convert.ToInt32(dr["Total"]) : 0;
                    lstchart.Add(objchart);
                }

            }
            return lstchart;
        }
        public List<DTO.DTOCompanyList> GetCompanyList()
        {
            List<DTO.DTOCompanyList> _HolidayLst = new List<DTO.DTOCompanyList>();
            var dt = objDb.ExecuteReader("[sp_GetcompanyName]");
            if (dt.Rows.Count > 0)
            {
                _HolidayLst = Utility.CommonFuction.ConvertDataTableToList<DTO.DTOCompanyList>(dt);
            }
            return _HolidayLst;
        }
        public List<DTO.ClosingCalanderList> GetCompanyClosingCalList()
        {
            List<DTO.ClosingCalanderList> _HolidayLst = new List<DTO.ClosingCalanderList>();
            var dt = objDb.ExecuteReader("[sp_getclosingcallist]");
            if (dt.Rows.Count > 0)
            {
                _HolidayLst = Utility.CommonFuction.ConvertDataTableToList<DTO.ClosingCalanderList>(dt);
            }
            return _HolidayLst;
        }
        public List<EmailSendingDto> PostCompnayWithTime(CompanyWiseDate model)
        {
            try
            {
                List<EmailSendingDto> lst = new List<EmailSendingDto>();
                List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@CompanyId", SqlDbType=SqlDbType.BigInt, Value=model.CompanyId },
              new SqlParameter{ ParameterName ="@ClosingDate",SqlDbType=SqlDbType.DateTime,Value=model.CloseDate}
            };
                var dt = objDb.ExecuteReader("[SP_PostCompWiseData]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        EmailSendingDto obj = new EmailSendingDto();
                        obj.TaskId = dr.Table.Columns.Contains("TaskId") ? Convert.ToInt32(dr["TaskId"].ToString()) : 0;
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.CompanyName = dr.Table.Columns.Contains("CompanyName") ? Convert.ToString(dr["CompanyName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.EmailId = dr.Table.Columns.Contains("EmailId") ? Convert.ToString(dr["EmailId"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.UserName = dr.Table.Columns.Contains("UserName") ? Convert.ToString(dr["UserName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.MonthYear = dr.Table.Columns.Contains("MonthYear") ? Convert.ToString(dr["MonthYear"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Duedateforsubmission = dr.Table.Columns.Contains("Duedateforsubmission") ? Convert.ToString(dr["Duedateforsubmission"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Duedateforapproval = dr.Table.Columns.Contains("Duedateforapproval") ? Convert.ToString(dr["Duedateforapproval"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.TaskName = dr.Table.Columns.Contains("TaskName") ? Convert.ToString(dr["TaskName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Template = dr.Table.Columns.Contains("Template") ? Convert.ToString(dr["Template"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.

                        lst.Add(obj);
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GlobalCalendar.MVC.DTO.TaskModel> GetList(string EmpCode)
        {
            try
            {
                List<GlobalCalendar.MVC.DTO.TaskModel> lstTask = new List<DTO.TaskModel>();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@DomainId",SqlDbType=SqlDbType.NVarChar,Value=EmpCode}
                };
                var dt = objDb.ExecuteReader("[sp_taskdomainidwiselist]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstTask = Utility.CommonFuction.ConvertDataTableToList<DTO.TaskModel>(dt);
                }
                return lstTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<GlobalCalendar.MVC.DTO.ListApproveTask> GetApproveList(string EmpCode)
        {
            try
            {
                List<GlobalCalendar.MVC.DTO.ListApproveTask> lstTask = new List<DTO.ListApproveTask>();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@DomainId",SqlDbType=SqlDbType.NVarChar,Value=EmpCode}
                };
                var dt = objDb.ExecuteReader("[sp_TaskApproveListByDomainId]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstTask = Utility.CommonFuction.ConvertDataTableToList<DTO.ListApproveTask>(dt);
                }
                return lstTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int PostBroadCastMes(BroadcastDTo model)
        {
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter{ParameterName="@Type",SqlDbType=SqlDbType.Int,Value=model.Type},
                    new SqlParameter{ParameterName="@Message",SqlDbType=SqlDbType.NVarChar,Value=model.Message}
                };
                var dt = objDb.ExecuteReader("[sp_BrodcastMessage]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["Id"]);
                }
                return 0;
               }
            catch
            {
                return 0;
            }
        }
        public bool UploadFile(string FileName,int rept)
        {
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@imagename",SqlDbType=SqlDbType.NVarChar,Value=FileName},
                    new SqlParameter{ParameterName="@typeid",SqlDbType=SqlDbType.Int,Value=rept}
                };
                var dt = objDb.ExecuteReader("[sp_imageupload]", lstparam);
                return true;
            }
            catch 
            {

                return false;
            }
        }
        public List<GlobalCalendar.MVC.DTO.ListApproveTask> GetApprovedList(string EmpCode)
        {
            try
            {
                List<GlobalCalendar.MVC.DTO.ListApproveTask> lstTask = new List<DTO.ListApproveTask>();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@DomainId",SqlDbType=SqlDbType.NVarChar,Value=EmpCode}
                };
                var dt = objDb.ExecuteReader("[sp_TaskApprovedListByDomainId]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstTask = Utility.CommonFuction.ConvertDataTableToList<DTO.ListApproveTask>(dt);
                }
                return lstTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<GlobalCalendar.MVC.DTO.ReasonMaster> GetReason()
        {
            try
            {
                List<GlobalCalendar.MVC.DTO.ReasonMaster> lstReason = new List<ReasonMaster>();
                var dt = objDb.ExecuteReader("[sp_getreason]");
                if (dt.Rows.Count > 0)
                {
                    lstReason = Utility.CommonFuction.ConvertDataTableToList<ReasonMaster>(dt);
                }
                return lstReason;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GlobalCalendar.MVC.DTO.TaskDetail GetTaskDetailById(int EmpCode)
        {
            try
            {
                GlobalCalendar.MVC.DTO.TaskDetail lstTask = new GlobalCalendar.MVC.DTO.TaskDetail();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@TaskId",SqlDbType=SqlDbType.NVarChar,Value=EmpCode}
                };
                var dt = objDb.ExecuteReader("[SP_TaskDetailById]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstTask.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    lstTask.dtId = Convert.ToInt32(dt.Rows[0]["Id"]);
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.TaskName = Convert.ToString(dt.Rows[0]["TaskName"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.TaskDescription = Convert.ToString(dt.Rows[0]["TaskDescription"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Period = Convert.ToString(dt.Rows[0]["Period"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.PrimaryOwner = Convert.ToString(dt.Rows[0]["PrimaryOwner"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.SecondaryOwner = Convert.ToString(dt.Rows[0]["SecondaryOwner"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.PrimaryApprover = Convert.ToString(dt.Rows[0]["PrimaryApprover"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.SecondaryApprover = Convert.ToString(dt.Rows[0]["SecondaryApprover"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Comment = Convert.ToString(dt.Rows[0]["Comment"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.UploadFile = Convert.ToString(dt.Rows[0]["UploadFile"]);
#pragma warning restore CS8601 // Possible null reference assignment.
                    lstTask.OverdueTask = Convert.ToInt32(dt.Rows[0]["OverdueTask"]);
                    lstTask.TaskDocId = Convert.ToInt32(dt.Rows[0]["TaskDocId"]);
                    lstTask.IsSubmitTask = Convert.ToBoolean(dt.Rows[0]["IsSubmitTask"]);
                    lstTask.Duedateforsubmission = Convert.ToDateTime(dt.Rows[0]["Duedateforsubmission"]);
                }
                return lstTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public GlobalCalendar.MVC.DTO.TaskDetail GETTaskDetailApprover(int EmpCode)
        {
            try
            {
                GlobalCalendar.MVC.DTO.TaskDetail lstTask = new GlobalCalendar.MVC.DTO.TaskDetail();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                    new SqlParameter{ParameterName="@TaskId",SqlDbType=SqlDbType.NVarChar,Value=EmpCode}
                };
                var dt = objDb.ExecuteReader("[SP_TaskDetailByApprovalId]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstTask.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    lstTask.dtId = Convert.ToInt32(dt.Rows[0]["Id"]);
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.TaskName = Convert.ToString(dt.Rows[0]["TaskName"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.TaskDescription = Convert.ToString(dt.Rows[0]["TaskDescription"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Period = Convert.ToString(dt.Rows[0]["Period"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.PrimaryOwner = Convert.ToString(dt.Rows[0]["PrimaryOwner"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.SecondaryOwner = Convert.ToString(dt.Rows[0]["SecondaryOwner"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.PrimaryApprover = Convert.ToString(dt.Rows[0]["PrimaryApprover"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.SecondaryApprover = Convert.ToString(dt.Rows[0]["SecondaryApprover"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Comment = Convert.ToString(dt.Rows[0]["Comment"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.UploadFile = Convert.ToString(dt.Rows[0]["UploadFile"]);
#pragma warning restore CS8601 // Possible null reference assignment.
                    lstTask.OverdueTask = Convert.ToInt32(dt.Rows[0]["OverdueTask"]);
                    lstTask.TaskDocId = Convert.ToInt32(dt.Rows[0]["TaskDocId"]);
                    lstTask.IsSubmitTask = Convert.ToBoolean(dt.Rows[0]["IsSubmitTask"]);
                    lstTask.Duedateforsubmission = Convert.ToDateTime(dt.Rows[0]["Duedateforsubmission"]);
                }
                return lstTask;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool UpdateStatus(TempPut objput)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                       new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=objput.Id },
                       new SqlParameter { ParameterName="@AcceptTask", SqlDbType=SqlDbType.DateTime, Value=objput.Date },
                       new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=objput.DomainId },

                };
                var dt = objDb.ExecuteReader("[sp_updatetaskdetail]", lstparam);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public bool UpdateSubmit(TempPut objput)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                       new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=objput.Id },
                       new SqlParameter { ParameterName="@AcceptTask", SqlDbType=SqlDbType.DateTime, Value=objput.Date },
                       new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=objput.DomainId },

                };
                var dt = objDb.ExecuteReader("[sp_submit]", lstparam);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public bool UpdateApprove(TempPut objput)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                       new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=objput.Id },
                       new SqlParameter { ParameterName="@AcceptTask", SqlDbType=SqlDbType.DateTime, Value=objput.Date },
                       new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=objput.DomainId },

                };
                var dt = objDb.ExecuteReader("[sp_updateApproveDetail]", lstparam);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }
        public EmailTemplateSender SaveTaskDetail(TaskDetail obj)
        {
            try
            {
                EmailTemplateSender lstTask = new EmailTemplateSender();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@Id", SqlDbType=SqlDbType.Int, Value=obj.Id },
                     new SqlParameter { ParameterName="@TaskDocId", SqlDbType=SqlDbType.Int, Value=obj.TaskDocId },
                     new SqlParameter { ParameterName="@Comment", SqlDbType=SqlDbType.NVarChar, Value=obj.Comment },
                     new SqlParameter { ParameterName="@OverdueTask", SqlDbType=SqlDbType.Int, Value=obj.OverdueTask },
                     new SqlParameter { ParameterName="@UploadFile", SqlDbType=SqlDbType.NVarChar, Value=obj.UploadFile },
                     new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=obj.DomainId }

                };
                var dt = objDb.ExecuteReader("[SP_SaveDoc]", lstparam);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // Assign values from the DataRow to the TaskViewModel properties
                    lstTask.TaskId = Convert.ToInt32(row["TaskId"]);
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.EmailId = row["EmailId"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.UserName = row["UserName"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.AdminEmailId = row["AdminEmailId"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.AdminUserName = row["AdminUserName"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.CompanyName = row["CompanyName"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.MonthYear = row["MonthYear"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.TaskName = row["TaskName"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Duedateforsubmission = row["Duedateforsubmission"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Duedateforapproval = row["Duedateforapproval"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Template1 = row["Template1"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    lstTask.Template2 = row["Template2"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                return lstTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmailDetail RejectApprover(TaskDetail obj)
        {
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=obj.Id },
                     new SqlParameter { ParameterName="@TaskDocId", SqlDbType=SqlDbType.Int, Value=obj.TaskDocId },
                     new SqlParameter { ParameterName="@Comment", SqlDbType=SqlDbType.NVarChar, Value=obj.Comment },
                     new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=obj.DomainId },

                };
                var EmailDetails = new EmailDetail();
                var dt = objDb.ExecuteReader("SP_UPDATEREJECT", lstparam);
                if (dt.Rows.Count > 0)
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    EmailDetails.Email = Convert.ToString(dt.Rows[0]["Email"]);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    EmailDetails.TaskName = Convert.ToString(dt.Rows[0]["TaskName"]);
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                return EmailDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FileDownloadView> GetFileList(DTO.UploadFile upload)
        {
            try
            {
                List<FileDownloadView> lstFileDownloadView = new List<FileDownloadView>();
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=upload.TaskId },
                     new SqlParameter { ParameterName="@TaskDocumnet", SqlDbType=SqlDbType.NVarChar, Value=upload.TaskDocumnet },
                     new SqlParameter { ParameterName="@TaskFileName", SqlDbType=SqlDbType.NVarChar, Value=upload.TaskFileName },

                };
                var dt = objDb.ExecuteReader("SP_UPLOADFILE", lstparam);
                if (dt.Rows.Count > 0)
                {
                    lstFileDownloadView = Utility.CommonFuction.ConvertDataTableToList<FileDownloadView>(dt);
                }
                return lstFileDownloadView;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteRecord(int no)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@UploadedId", SqlDbType=SqlDbType.Int, Value=no}
                };
                var dt = objDb.ExecuteReader("SP_DELETEUPLOADEDDOC", lstparam);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
#pragma warning restore CS0168 // Variable is declared but never used

        }
        public List<FileDownloadView> GetUploadFileList(int Id)
        {
            List<FileDownloadView> lstFileDownloadview = new List<FileDownloadView>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=Id}
                };
            var dt = objDb.ExecuteReader("SP_DOCUMENTDETAILBYID", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstFileDownloadview = Utility.CommonFuction.ConvertDataTableToList<FileDownloadView>(dt);
            }
            return lstFileDownloadview;
        }
        public List<DTOComment> GetAllComment(int id)
        {
            List<DTOComment> lstFileDownloadview = new List<DTOComment>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.Int, Value=id}
                };
            var dt = objDb.ExecuteReader("SP_ShowAllDocument", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstFileDownloadview = Utility.CommonFuction.ConvertDataTableToList<DTOComment>(dt);
            }
            return lstFileDownloadview;
        }
        public List<YearMasterDto> GetListOfYear()
        {
            List<YearMasterDto> lstYear = new List<YearMasterDto>();
            var dt = objDb.ExecuteReader("sp_yeardetails");
            if (dt.Rows.Count > 0)
            {
                lstYear = Utility.CommonFuction.ConvertDataTableToList<YearMasterDto>(dt);
            }
            return lstYear;
        }
        public List<CompanyMaster> GetAllCompanyList()
        {
            List<CompanyMaster> lstYear = new List<CompanyMaster>();
            var dt = objDb.ExecuteReader("SP_CompanyDetail");
            if (dt.Rows.Count > 0)
            {
                lstYear = Utility.CommonFuction.ConvertDataTableToList<CompanyMaster>(dt);
            }
            return lstYear;
        }
        public List<TaskSummaryViewModel> GetCompanyWiseReport(int year, int companyid, int Month)
        {
            List<TaskSummaryViewModel> lstTask = new List<TaskSummaryViewModel>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("SP_Companycodewiselist", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<TaskSummaryViewModel>(dt);
            }
            return lstTask;
        }
        public List<ErrorReportDto> GetErrorReport(int year, int companyid, int Month)
        {
            List<ErrorReportDto> lstTask = new List<ErrorReportDto>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("sp_errorreport", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<ErrorReportDto>(dt);
            }
            return lstTask;
        }
        public List<OwnerTaskSummary> GetOwnerWiseReport(int year, int companyid, int Month)
        {
            List<OwnerTaskSummary> lstTask = new List<OwnerTaskSummary>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("SP_OwnerWise", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<OwnerTaskSummary>(dt);
            }
            return lstTask;
        }
        public List<TaskResourceRept> GetTaskResourceRepts(int year, int companyid, int Month)
        {
            List<TaskResourceRept> lstTask = new List<TaskResourceRept>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("[SP_RESOURSEWISELIST]", lstparam);
            if (dt.Rows.Count > 0)
            {
                /*lstTask = Utility.CommonFuction.ConvertDataTableToList<TaskResourceRept>(dt);*/
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    TaskResourceRept rept = new TaskResourceRept();
                    int Approvedtask = 0;
                    rept.EmpName = dr.Table.Columns.Contains("EmpName") ? Convert.ToString(dr["EmpName"].ToString()) : String.Empty;
                    rept.Company = dr.Table.Columns.Contains("Company") ? Convert.ToString(dr["Company"].ToString()) : String.Empty;
                    rept.Period = dr.Table.Columns.Contains("Period") ? Convert.ToString(dr["Period"].ToString()) : String.Empty;

                    rept.TotalTask = dr.Table.Columns.Contains("TotalTask") ? Convert.ToInt32(dr["TotalTask"]) : 0;
                   /* Approvedtask = dr.Table.Columns.Contains("Percentage") ? Convert.ToInt32(dr["Percentage"]) : 0;
                  
                    var per = (Approvedtask * 100) / rept.TotalTask;*/
                  
                    rept.Percentage = dr.Table.Columns.Contains("Percentage") ? Convert.ToString(dr["Percentage"].ToString()) : String.Empty;
                    lstTask.Add(rept);
                }
            }
            return lstTask;
        }
        public List<OwnerDetailList> GetOwnerWiseReport(int year, int companyid, int Month, string employeename)
        {
            List<OwnerDetailList> lstTask = new List<OwnerDetailList>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month},
                     new SqlParameter { ParameterName="@EmpName", SqlDbType=SqlDbType.NVarChar, Value=employeename}
                };
            var dt = objDb.ExecuteReader("SP_OwnerWiseDetail", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<OwnerDetailList>(dt);
            }
            return lstTask;
        }
        public List<OwnerDetailList> GetApproveWiseReport(int year, int companyid, int Month, string employeename)
        {
            List<OwnerDetailList> lstTask = new List<OwnerDetailList>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month},
                     new SqlParameter { ParameterName="@EmpName", SqlDbType=SqlDbType.NVarChar, Value=employeename}
                };
            var dt = objDb.ExecuteReader("SP_ApproverWiseDetail", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<OwnerDetailList>(dt);
            }
            return lstTask;
        }
        public List<ApproverTaskSummary> GetApproveList(int year, int companyid, int Month)
        {
            List<ApproverTaskSummary> lstTask = new List<ApproverTaskSummary>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("SP_ApproverWise", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<ApproverTaskSummary>(dt);
            }
            return lstTask;
        }
        public List<TaskSummaryChart> GetSubmitTaskChart(string DomainId)
        {
            List<TaskSummaryChart> lstTask = new List<TaskSummaryChart>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=DateTime.Now.Year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=687},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=DateTime.Now.Month},
                     new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=DomainId}
                };
            var dt = objDb.ExecuteReader("SP_OwnerWiseDetailByDomainId", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<TaskSummaryChart>(dt);
            }
            return lstTask;
        }
        public List<TaskSummaryChart> GetApproveTaskChart(string DomainId)
        {
            List<TaskSummaryChart> lstTask = new List<TaskSummaryChart>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=DateTime.Now.Year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=687},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=DateTime.Now.Month},
                     new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=DomainId}
                };
            var dt = objDb.ExecuteReader("SP_ApproverWiseDetailByDomainId", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<TaskSummaryChart>(dt);
            }
            return lstTask;
        }
        public List<ExceptionViewModel> GetExceptionList(int year, int companyid, int Month)
        {
            List<ExceptionViewModel> lstTask = new List<ExceptionViewModel>();
            List<SqlParameter> lstparam = new List<SqlParameter>()
                {
                     new SqlParameter { ParameterName="@year", SqlDbType=SqlDbType.BigInt, Value=year},
                     new SqlParameter { ParameterName="@companyid", SqlDbType=SqlDbType.BigInt, Value=companyid},
                     new SqlParameter { ParameterName="@Month", SqlDbType=SqlDbType.Int, Value=Month}
                };
            var dt = objDb.ExecuteReader("SP_Exception", lstparam);
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<ExceptionViewModel>(dt);
            }
            return lstTask;
        }
        public bool UpdateViewStatus(int id)
        {
            try {
                List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@id", SqlDbType=SqlDbType.Int, Value=id}
                };
                var dt = objDb.ExecuteReader("sp_updateview", lstparam);
                return true;
            }
            catch { return false; }
        }
        public bool UploadHistoryExcel(string dfilename,string fname)
        {
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@FileName", SqlDbType=SqlDbType.NVarChar, Value=fname},
                    new SqlParameter { ParameterName="@DownloadFileName", SqlDbType=SqlDbType.NVarChar, Value=dfilename}
                };
                var dt = objDb.ExecuteReader("sp_excelhistory", lstparam);
                return true;
            }
            catch 
            {

                return false;
            }
        }
        public List<BroadcastDTo> GetBroadcastMessage()
        {
            List<BroadcastDTo> lstTask = new List<BroadcastDTo>();
         
            var dt = objDb.ExecuteReader("SP_BroadcastMessage");
            if (dt.Rows.Count > 0)
            {
                lstTask = Utility.CommonFuction.ConvertDataTableToList<BroadcastDTo>(dt);
            }
            return lstTask;
        }
        public int GetTotalBroadcastNoti()
        {
            int total = 0;
         
            var dt = objDb.ExecuteReader("SP_BroadcastMessagecount");
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                total = Convert.ToInt32(row["TotalNotification"]);
            }
            return total;
            }
        public string CheckFileExsists(DTO.UploadFile upload)
        {
            string ExsistsFile = string.Empty;
            List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.BigInt, Value=upload.TaskId},
                     new SqlParameter { ParameterName="@TaskFileName", SqlDbType=SqlDbType.NVarChar, Value=upload.TaskFileName}

                };
            var dt = objDb.ExecuteReader("sp_filecheck", lstparam);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                // Assign values from the DataRow to the TaskViewModel properties
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                ExsistsFile = Convert.ToString(row["TaskDocumnet"]);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return ExsistsFile;
#pragma warning restore CS8603 // Possible null reference return.
        }
        public List<TaskTrigger> GetTriggerList()
        {
            List<TaskTrigger> taskTriggers = new List<TaskTrigger>();
            var dt = objDb.ExecuteReader("SP_SENDINGEMAILSEDUILAR");
            if (dt.Rows.Count > 0)
            {
                taskTriggers = Utility.CommonFuction.ConvertDataTableToList<TaskTrigger>(dt);
            }
            return taskTriggers;
        }
        public List<MatreixModel> GetMatrixList()
        {
            List<MatreixModel> taskTriggers = new List<MatreixModel>();
            var dt = objDb.ExecuteReader("SP_MatrixList");
            if (dt.Rows.Count > 0)
            {
                taskTriggers = Utility.CommonFuction.ConvertDataTableToList<MatreixModel>(dt);
            }
            return taskTriggers;
        }
        public List<User> GetUser()
        {
            List<User> taskTriggers = new List<User>();
            var dt = objDb.ExecuteReader("sp_employeelist");
            if (dt.Rows.Count > 0)
            {
                taskTriggers = Utility.CommonFuction.ConvertDataTableToList<User>(dt);
            }
            return taskTriggers;
        }
        public UpdateMatrixDetail GetMatrixDeatil(int TaskId)
        {
         UpdateMatrixDetail taskTriggers = new UpdateMatrixDetail();
            List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.BigInt, Value=Convert.ToInt64(TaskId)},
                     

                };
            var dt = objDb.ExecuteReader("SP_TaskIdGetAllUserList",lstparam);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                taskTriggers.TaskId = Convert.ToInt64(row["TaskId"]);
                taskTriggers.PrimayOwnerDomainId = Convert.ToString(row["PrimayOwnerDomainId"]);
                taskTriggers.SecondaryOwnerDomainId = Convert.ToString(row["SecondaryOwnerDomainId"]);
                taskTriggers.PrimaryAproverDomainId = Convert.ToString(row["PrimaryAproverDomainId"]);
                taskTriggers.SecondaryApproverDomainId = Convert.ToString(row["SecondaryApproverDomainId"]);
                taskTriggers.IsSubmitTask = Convert.ToBoolean(row["IsSubmmit"]);
            }
            return taskTriggers;
        }
        public void DelleteBroadcasthistory(int id)
        {
            List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@id", SqlDbType=SqlDbType.Int, Value=id},
            

                };
            var dt = objDb.ExecuteReader("SP_DELTEBROADCASTHISTORY", lstparam);

        }
        public void InsertHistory(string RoleId,int Roomno)
        {
            List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@RoleId", SqlDbType=SqlDbType.BigInt, Value=Convert.ToInt32(RoleId)},
                    new SqlParameter { ParameterName="@RoomId", SqlDbType=SqlDbType.NVarChar, Value=Convert.ToInt32(Roomno)}
                   

                };
            var dt = objDb.ExecuteReader("SP_AddBroadcastHistory", lstparam);

        }
        public List<EmailSendingDto> UpdateMatrix(UpdateMatrixDetail Model)
        {
            try
            {
                List<EmailSendingDto> lst=new List<EmailSendingDto>();
                List<SqlParameter> lstparam = new List<SqlParameter>()
            {
                    new SqlParameter { ParameterName="@TaskId", SqlDbType=SqlDbType.BigInt, Value=Convert.ToInt64(Model.TaskId)},
                    new SqlParameter { ParameterName="@PrimaryOwnerDomainId", SqlDbType=SqlDbType.NVarChar, Value=Convert.ToInt64(Model.PrimayOwnerDomainId)},
                    new SqlParameter { ParameterName="@SecondaryOwnerDomainId", SqlDbType=SqlDbType.NVarChar, Value=Convert.ToInt64(Model.SecondaryOwnerDomainId)},
                    new SqlParameter { ParameterName="@PrimaryApproverDomainId", SqlDbType=SqlDbType.NVarChar, Value=Convert.ToInt64(Model.PrimaryAproverDomainId)},
                    new SqlParameter { ParameterName="@SecondaryApproverDomainId", SqlDbType=SqlDbType.NVarChar, Value=Convert.ToInt64(Model.SecondaryApproverDomainId)},


                };
                var dt = objDb.ExecuteReader("SP_UpdateMatrix", lstparam);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        EmailSendingDto obj = new EmailSendingDto();
                        obj.TaskId = dr.Table.Columns.Contains("TaskId") ? Convert.ToInt32(dr["TaskId"].ToString()) : 0;
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.CompanyName = dr.Table.Columns.Contains("CompanyName") ? Convert.ToString(dr["CompanyName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.EmailId = dr.Table.Columns.Contains("EmailId") ? Convert.ToString(dr["EmailId"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.UserName = dr.Table.Columns.Contains("UserName") ? Convert.ToString(dr["UserName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.MonthYear = dr.Table.Columns.Contains("MonthYear") ? Convert.ToString(dr["MonthYear"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Duedateforsubmission = dr.Table.Columns.Contains("Duedateforsubmission") ? Convert.ToString(dr["Duedateforsubmission"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Duedateforapproval = dr.Table.Columns.Contains("Duedateforapproval") ? Convert.ToString(dr["Duedateforapproval"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.TaskName = dr.Table.Columns.Contains("TaskName") ? Convert.ToString(dr["TaskName"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        obj.Template = dr.Table.Columns.Contains("Template") ? Convert.ToString(dr["Template"].ToString()) : String.Empty;
#pragma warning restore CS8601 // Possible null reference assignment.

                        lst.Add(obj);
                    }
                }
                return lst;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        } 
    }
}
