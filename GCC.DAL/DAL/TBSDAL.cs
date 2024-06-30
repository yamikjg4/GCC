using GlobalCalendar.DAL;
using GlobalCalendar.MVC.Models;
using System.Data;
using System.Data.SqlClient;

namespace GlobalCalendar.MVC.DAL
{
    public class TBSDAL
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        GlobalCalendar.DAL.DBConnection objDb = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /*     SAPDBConnection objSAPDb = null;*/
        public TBSDAL()
        {
            objDb = new DBConnection();
            /*  objSAPDb = new SAPDBConnection();*/
        }

        public List<string> GetRequestType()
        {
            List<SqlParameter> lstparam = new List<SqlParameter>() { };
            List<string> lstRequestType = new List<string>();
            lstRequestType.Add("Select");
            var dt = objDb.ExecuteReader("[USP_Get_REQUEST_TYPE]", lstparam);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
#pragma warning disable CS8604 // Possible null reference argument.
                    lstRequestType.Add(dr.Table.Columns.Contains("RequestType") ? dr["RequestType"].ToString() : null);
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }
            return lstRequestType;
        }

        /*public List<IOSearchModel> GetAllInternalOrders(string companyCode, string orderType, string orderDescription, string orderNumber)
        {
            List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@companyCode", SqlDbType=SqlDbType.NVarChar, Value=companyCode },
              new SqlParameter { ParameterName="@orderType", SqlDbType=SqlDbType.NVarChar, Value=orderType },
              new SqlParameter { ParameterName="@orderDescription", SqlDbType=SqlDbType.NVarChar, Value=orderDescription },
              new SqlParameter { ParameterName="@orderNumber", SqlDbType=SqlDbType.NVarChar, Value=orderNumber }
            };

            List<IOSearchModel> iOSearchModelsList = new List<IOSearchModel>();
            var dt = objSAPDb.ExecuteReader("[GetTBSInternalOrders]", lstparam);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    IOSearchModel iOSearch = new IOSearchModel();

                    iOSearch.Order = dr["AUFNR"].ToString();
                    iOSearch.Order_Type = dr["AUART"].ToString();
                    iOSearch.CO_Area = dr["KOKRS"].ToString();
                    iOSearch.Description = dr["KTEXT"].ToString();

                    iOSearchModelsList.Add(iOSearch);
                }
            }
            return iOSearchModelsList;
        }
*/
        public List<UserInfo> GetUserAuthorisation(string emailId)
        {
            List<SqlParameter> lstparam = new List<SqlParameter>() {
              new SqlParameter { ParameterName="@DomainId", SqlDbType=SqlDbType.NVarChar, Value=emailId }
            };

            List<UserInfo> _userInfoList = new List<UserInfo>();
            var dt = objDb.ExecuteReader("[SP_GETUSERDATA]", lstparam);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    UserInfo _userInfo = new UserInfo();
                    if (dr != null)
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.userId = dr["Id"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.RoleId = dr["RoleId"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.EmpCode = dr["DomainId"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.Name = dr["Employee_Name"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.Segment = dr["Segment"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                        _userInfo.EmailId = dr["Email_ID"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                        _userInfoList.Add(_userInfo);
                    }
                }
            }
            return _userInfoList;
        }

        public UserInfo GetEmployeeCodeAndName(string emailId)
        {
            UserInfo _userInfo = new UserInfo();
            //empCode = 0;
            //empName = string.Empty;
            try
            {
                var employeeTable = GetEmployeeBasic(string.Empty, string.Empty, emailId);

                if (employeeTable != null && employeeTable.Rows.Count > 0)
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userInfo.EmpCode = employeeTable.Rows[0]["EmpCode"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userInfo.Name = employeeTable.Rows[0]["EmpName"].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _userInfo;
        }

        private static DataTable GetEmployeeBasic(string empCode, string domainId, string emailId)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            DataTable dataTable = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var parameters = new Dictionary<string, object>();
            parameters.Add("@empCode", empCode);
            parameters.Add("@emailId", emailId);
            parameters.Add("@domainId", domainId);

            var data = SQLSERVER_HELPER.GetSAPDBData("[dbo].[GetEmployeeBasic]", parameters);

            if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
                dataTable = data.Tables[0];

#pragma warning disable CS8603 // Possible null reference return.
            return dataTable;
#pragma warning restore CS8603 // Possible null reference return.
        }

        /*public List<TimesheetModel> GetTimeSheet(string empCode, string dateFrom)
        {
            List<TimesheetModel> _TimesheetModelList = new List<TimesheetModel>();
            DataTable dt = new DataTable();

            dt = GetTimesheet(empCode, dateFrom);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                TimesheetModel _timesheetModel = new TimesheetModel();
                _timesheetModel.Id = dr.Table.Columns.Contains("Id") ? Convert.ToInt64(dr["Id"].ToString()) : 0;
                _timesheetModel.UserId = dr.Table.Columns.Contains("UserId") ? Convert.ToInt32(dr["UserId"].ToString()) : 0;
                _timesheetModel.EmpCode = dr.Table.Columns.Contains("EmpCode") ? dr["EmpCode"].ToString() : null;
                _timesheetModel.OrderId = dr.Table.Columns.Contains("OrderId") ? dr["OrderId"].ToString() : null;
                _timesheetModel.OrderDescription = dr.Table.Columns.Contains("OrderDescription") ? dr["OrderDescription"].ToString() : null;
                _timesheetModel.ActivityType = dr.Table.Columns.Contains("ActivityType") ? dr["ActivityType"].ToString() : null;
                _timesheetModel.ActivityDescription = dr.Table.Columns.Contains("ActivityDescription") ? dr["ActivityDescription"].ToString() : null;
                _timesheetModel.Remark = dr.Table.Columns.Contains("Remark") ? dr["Remark"].ToString() : null;
                //_timesheetModel.StartTime = dr.Table.Columns.Contains("StartTime") ? dr["StartTime"].ToString() : null;
                //_timesheetModel.EndTime = dr.Table.Columns.Contains("EndTime") ? dr["EndTime"].ToString() : null;
                _timesheetModel.Hrs = dr.Table.Columns.Contains("Hrs") ? dr["Hrs"].ToString() : null;
                _timesheetModel.Date = dr.Table.Columns.Contains("Date") ? dr["Date"].ToString() : null;
                _timesheetModel.IsSubmited = true;

                _TimesheetModelList.Add(_timesheetModel);
            }
            return _TimesheetModelList;
        }
        
        public List<TimesheetModel> GetTimeSheetSave(string empCode, string dateFrom)
        {
            List<TimesheetModel> _TimesheetModelList = new List<TimesheetModel>();
            DataTable dt = new DataTable();

            dt = GetTimesheetSave(empCode, dateFrom);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                TimesheetModel _timesheetModel = new TimesheetModel();
                _timesheetModel.Id = dr.Table.Columns.Contains("Id") ? Convert.ToInt64(dr["Id"].ToString()) : 0;
                _timesheetModel.UserId = dr.Table.Columns.Contains("UserId") ? Convert.ToInt32(dr["UserId"].ToString()) : 0;
                _timesheetModel.EmpCode = dr.Table.Columns.Contains("EmpCode") ? dr["EmpCode"].ToString() : null;
                _timesheetModel.OrderId = dr.Table.Columns.Contains("OrderId") ? dr["OrderId"].ToString() : null;
                _timesheetModel.OrderDescription = dr.Table.Columns.Contains("OrderDescription") ? dr["OrderDescription"].ToString() : null;
                _timesheetModel.ActivityType = dr.Table.Columns.Contains("ActivityType") ? dr["ActivityType"].ToString() : null;
                _timesheetModel.ActivityDescription = dr.Table.Columns.Contains("ActivityDescription") ? dr["ActivityDescription"].ToString() : null;
                _timesheetModel.Remark = dr.Table.Columns.Contains("Remark") ? dr["Remark"].ToString() : null;
                //_timesheetModel.StartTime = dr.Table.Columns.Contains("StartTime") ? dr["StartTime"].ToString() : null;
                //_timesheetModel.EndTime = dr.Table.Columns.Contains("EndTime") ? dr["EndTime"].ToString() : null;
                _timesheetModel.Hrs = dr.Table.Columns.Contains("Hrs") ? dr["Hrs"].ToString() : null;
                _timesheetModel.Date = dr.Table.Columns.Contains("Date") ? dr["Date"].ToString() : null;
                _timesheetModel.IsSubmited = false;

                _TimesheetModelList.Add(_timesheetModel);
            }
            return _TimesheetModelList;
        }
        
        public List<ExportTimesheetModel> GetTimeSheetExport(string StartDate, string EndDate)
        {
            List<ExportTimesheetModel> _TimesheetModelList = new List<ExportTimesheetModel>();
            DataTable dt = new DataTable();

            dt = GetTimesheetExportDt(StartDate, EndDate);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                ExportTimesheetModel _timesheetModel = new ExportTimesheetModel();
               
                _timesheetModel.Id = dr.Table.Columns.Contains("Id") ? Convert.ToInt64(dr["Id"].ToString()) : 0;              
                _timesheetModel.EmpCode = dr.Table.Columns.Contains("Employee_Code") ? dr["Employee_Code"].ToString() : null;
                _timesheetModel.Employee_Name = dr.Table.Columns.Contains("Employee_Name") ? dr["Employee_Name"].ToString() : null;
                _timesheetModel.Email_ID = dr.Table.Columns.Contains("Email_ID") ? dr["Email_ID"].ToString() : null;
                _timesheetModel.Segment = dr.Table.Columns.Contains("Segment") ? dr["Segment"].ToString() : null;
                _timesheetModel.OrderId = dr.Table.Columns.Contains("OrderId") ? dr["OrderId"].ToString() : null;
                _timesheetModel.OrderDescription = dr.Table.Columns.Contains("OrderDescription") ? dr["OrderDescription"].ToString() : null;
                _timesheetModel.ActivityType = dr.Table.Columns.Contains("ActivityType") ? dr["ActivityType"].ToString() : null;
                _timesheetModel.ActivityDescription = dr.Table.Columns.Contains("ActivityDescription") ? dr["ActivityDescription"].ToString() : null;
                _timesheetModel.Remark = dr.Table.Columns.Contains("Remark") ? dr["Remark"].ToString() : null;
                _timesheetModel.Date = dr.Table.Columns.Contains("Date") ? dr["Date"].ToString() : null;
                //_timesheetModel.StartTime = dr.Table.Columns.Contains("StartTime") ? dr["StartTime"].ToString() : null;
                //_timesheetModel.EndTime = dr.Table.Columns.Contains("EndTime") ? dr["EndTime"].ToString() : null;
                _timesheetModel.Hrs = dr.Table.Columns.Contains("Hrs") ? dr["Hrs"].ToString() : null;            

                _TimesheetModelList.Add(_timesheetModel);
            }
            return _TimesheetModelList;
        }
        
        public List<ExportTimesheetModel> GetTimeSheetExportUser(string EmpCode,string StartDate, string EndDate)
        {
            List<ExportTimesheetModel> _TimesheetModelList = new List<ExportTimesheetModel>();
            DataTable dt = new DataTable();

            dt = GetTimesheetExportDtUser(EmpCode,StartDate, EndDate);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                ExportTimesheetModel _timesheetModel = new ExportTimesheetModel();
               
                _timesheetModel.Id = dr.Table.Columns.Contains("Id") ? Convert.ToInt64(dr["Id"].ToString()) : 0;              
                _timesheetModel.EmpCode = dr.Table.Columns.Contains("Employee_Code") ? dr["Employee_Code"].ToString() : null;
                _timesheetModel.Employee_Name = dr.Table.Columns.Contains("Employee_Name") ? dr["Employee_Name"].ToString() : null;
                _timesheetModel.Email_ID = dr.Table.Columns.Contains("Email_ID") ? dr["Email_ID"].ToString() : null;
                _timesheetModel.Segment = dr.Table.Columns.Contains("Segment") ? dr["Segment"].ToString() : null;
                _timesheetModel.OrderId = dr.Table.Columns.Contains("OrderId") ? dr["OrderId"].ToString() : null;
                _timesheetModel.OrderDescription = dr.Table.Columns.Contains("OrderDescription") ? dr["OrderDescription"].ToString() : null;
                _timesheetModel.ActivityType = dr.Table.Columns.Contains("ActivityType") ? dr["ActivityType"].ToString() : null;
                _timesheetModel.ActivityDescription = dr.Table.Columns.Contains("ActivityDescription") ? dr["ActivityDescription"].ToString() : null;
                _timesheetModel.Remark = dr.Table.Columns.Contains("Remark") ? dr["Remark"].ToString() : null;
                _timesheetModel.Date = dr.Table.Columns.Contains("Date") ? dr["Date"].ToString() : null;
                //_timesheetModel.StartTime = dr.Table.Columns.Contains("StartTime") ? dr["StartTime"].ToString() : null;
                //_timesheetModel.EndTime = dr.Table.Columns.Contains("EndTime") ? dr["EndTime"].ToString() : null;
                _timesheetModel.Hrs = dr.Table.Columns.Contains("Hrs") ? dr["Hrs"].ToString() : null;            

                _TimesheetModelList.Add(_timesheetModel);
            }
            return _TimesheetModelList;
        }*/

        /*        private static DataTable GetTimesheet(string empCode, string _Date)
        *//*        {
                    DataTable dataTable = null;
                    var parameters = new Dictionary<string, object>();

                    parameters.Add("@empCode", empCode);
                    parameters.Add("@Date", _Date);

                    var data = SQLSERVER_HELPER.GetData("[dbo].[GetEmployeeTimesheet_New]", parameters);
                    if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
                        dataTable = data.Tables[0];

                    return dataTable;
                }
                private static DataTable GetTimesheetSave(string empCode, string _Date)
                {
                    DataTable dataTable = null;
                    var parameters = new Dictionary<string, object>();

                    parameters.Add("@empCode", empCode);
                    parameters.Add("@Date", _Date);

                    var data = SQLSERVER_HELPER.GetData("[dbo].[GetEmployeeTimesheet_New_Save]", parameters);
                    if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
                        dataTable = data.Tables[0];

                    return dataTable;
                }

                public DataTable GetTimesheetExportDt(string StartDate, string EndDate)
                {
                    DataTable dataTable = null;
                    var parameters = new Dictionary<string, object>();

                    parameters.Add("@StartDate", StartDate);
                    parameters.Add("@EndDate", EndDate);

                    var data = SQLSERVER_HELPER.GetData("[dbo].[GetEmployeeTimesheetExport_New]", parameters);
                    if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
                        dataTable = data.Tables[0];

                    return dataTable;
                }
                public DataTable GetTimesheetExportDtUser(string EmpCode,string StartDate, string EndDate)
                {
                    DataTable dataTable = null;
                    var parameters = new Dictionary<string, object>();

                    parameters.Add("@EmpCode", EmpCode);
                    parameters.Add("@StartDate", StartDate);
                    parameters.Add("@EndDate", EndDate);

                    var data = SQLSERVER_HELPER.GetData("[dbo].[GetEmployeeTimesheetExportUser_New]", parameters);
                    if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
                        dataTable = data.Tables[0];

                    return dataTable;
                }*/

        public List<UserMaster> GetAllAuthorisedUsers()
        {
            List<UserMaster> _userMasterList = new List<UserMaster>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var data = SQLSERVER_HELPER.GetData("[dbo].[GetAllAuthorisedUsers]", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            if (data != null && data.Tables.Count > 0 && data.Tables[0] != null)
            {
                DataTable dt = data.Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    UserMaster _userMaster = new UserMaster();
                    _userMaster.userId = dr.Table.Columns.Contains("ID") ? Convert.ToInt32(dr["ID"].ToString()) : 0;
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userMaster.DomainID = dr.Table.Columns.Contains("Employee_Code") ? dr["Employee_Code"].ToString() : null;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userMaster.Name = dr.Table.Columns.Contains("Employee_Name") ? dr["Employee_Name"].ToString() : null;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userMaster.Segment = dr.Table.Columns.Contains("Segment") ? dr["Segment"].ToString() : null;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                    _userMaster.EmailId = dr.Table.Columns.Contains("Email_ID") ? dr["Email_ID"].ToString() : null;
#pragma warning restore CS8601 // Possible null reference assignment.
                    _userMaster.Active = dr.Table.Columns.Contains("IS_ACTIVE") ? Convert.ToBoolean(dr["IS_ACTIVE"].ToString()) : false;

                    _userMasterList.Add(_userMaster);
                }
            }
            return _userMasterList;
        }

        public bool ActiveDeActiveUserAndRecords(int id, bool IsActive)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("@userId", id);
            parameters.Add("@IsActive", IsActive);
            var count = SQLSERVER_HELPER.CallStoredProcedure("[dbo].[ActiveDeActiveUserAndRecords]", parameters);
            return count > 0;
        }

        public bool AddNewAuthorisedUser(string domainId, string empName, string segment, string emailId, bool isActive)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("@domainId", domainId);
            parameters.Add("@empName", empName);
            parameters.Add("@segment", segment);
            parameters.Add("@emailId", emailId);
            parameters.Add("@isActive", isActive);

            var usersAdded = SQLSERVER_HELPER.CallStoredProcedure("[dbo].[AddNewAuthorisedUser]", parameters);
            return usersAdded > 0;
        }

        /*   public int SaveTimesheet(string jsonEntries,string _date, string DomainId)
           {
               var parameters = new Dictionary<string, object>();
               parameters.Add("@jsonTimesheet", jsonEntries);
               parameters.Add("@empCode", DomainId);
               parameters.Add("@Date", _date);

               var rowsAffected = SQLSERVER_HELPER.CallStoredProcedure("[dbo].[SaveEmployeeTimesheet_New]", parameters);
               return rowsAffected;
           }*/

        /*public int SubmitTimesheet(string jsonEntries,string Date,string DomainId)
        {
            //if (entryList == null || entryList.Count == 0)
            //    return 0;

            //var jsonEntries = JsonConvert.SerializeObject(entryList, Formatting.Indented);

            var parameters = new Dictionary<string, object>();
            parameters.Add("@jsonTimesheet", jsonEntries);
            parameters.Add("@empCode", DomainId);
            parameters.Add("@Date", Date);

            var rowsAffected = SQLSERVER_HELPER.CallStoredProcedure("[dbo].[SubmitEmployeeTimesheet_New]", parameters);
            return rowsAffected;
        }

        public ClsResponse DeleteTimesheetRecord(long Id)
        {
            ClsResponse clsResponse = new ClsResponse();
            try
            {
                List<SqlParameter> lstparam = new List<SqlParameter>() {
                  new SqlParameter { ParameterName="@Id", SqlDbType=SqlDbType.NVarChar, Value=Id},
                };

                clsResponse.Id = objDb.ExecuteScalar("DeleteTimesheet_New", lstparam);
                if (clsResponse.Id > 0)
                {
                    clsResponse.IsSuccess = true;
                    clsResponse.Msg = "Record removed successfully..!";
                }
                else
                {
                    clsResponse.IsSuccess = false;
                    clsResponse.Msg = "Something went wrong! ";
                }
            }
            catch (Exception e)
            {
                clsResponse.IsSuccess = false;
                clsResponse.Msg = "Something went wrong! " + e.Message;
            }

            return clsResponse;
        }*/

        /*  public List<TotalHrsModel> GetTotalHrs(string EmpCode, string DateFrom, string DateTo)
          {
              List<SqlParameter> lstparam = new List<SqlParameter>() {
                new SqlParameter { ParameterName="@empCode", SqlDbType=SqlDbType.NVarChar, Value=EmpCode },
                new SqlParameter { ParameterName="@dateFrom", SqlDbType=SqlDbType.NVarChar, Value=DateFrom },
                new SqlParameter { ParameterName="@dateTo", SqlDbType=SqlDbType.NVarChar, Value=DateTo }
              };

              List<TotalHrsModel> _TotalHrsModelList = new List<TotalHrsModel>();
              var dt = objDb.ExecuteReader("[GetEmployeeTotalHrs]", lstparam);

              if (dt.Rows.Count > 0)
              {
                  for (int i = 0; i < dt.Rows.Count; i++)
                  {
                      DataRow dr = dt.Rows[i];
                      TotalHrsModel _TotalHrsModel = new TotalHrsModel();
                      if (dr != null)
                      {
                          _TotalHrsModel.EmpCode = dr["EmpCode"].ToString();
                          _TotalHrsModel.Hrs = dr["TotalHrs"].ToString();
                          _TotalHrsModel.Date = dr["Date"].ToString();
                          _TotalHrsModelList.Add(_TotalHrsModel);
                      }
                  }
              }
              return _TotalHrsModelList;
          }*/


    }//Close of class

}//close of namespace