using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.DirectoryServices;
using System.Security.Principal;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace GlobalCalendar.MVC.Controllers
{
    
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly TBSDAL _tbsDAL;
        private readonly ILogger<LoginController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, TBSDAL tBSDAL, ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _tbsDAL = tBSDAL;
            _logger = logger;
        }
        /// <summary>
        /// Login View Page
        /// </summary>
        /// <param name="isDiffUser"></param>
        /// <param name="DomainId"></param>
        /// <returns></returns>
        public IActionResult Index(bool isDiffUser = false, string DomainId = "")
        {
            /*  Utility.SMTPService.SendEmail("Yamik.Mindnerves@suzlon.com","Try","Try Email");*/
            if (TempData["ResultMessage"] != null)
            {
                ViewBag.ResultMsg = TempData["ResultMessage"];
                TempData["ResultMessage"] = null;
            }

            if (string.IsNullOrEmpty(DomainId) && !isDiffUser)
            {
                DomainId = getUser();
                if (!string.IsNullOrEmpty(DomainId))
                    return autoLogin(DomainId);

            }
            else if (!string.IsNullOrEmpty(DomainId))
            {
                return autoLogin(DomainId);
            }

            return View(new UserInfo());
        }
        /// <summary>
        /// AutoLogin On DomainId
        /// </summary>
        /// <param name="DomainId"></param>
        /// <returns></returns>
        ActionResult autoLogin(string DomainId)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.DomainID = DomainId;
            userInfo.Password = _configuration["MasterPwd"];
            return ValidateUser(userInfo);
        }
        /// <summary>
        /// Login User Method
        /// </summary>
        /// <param name="_userInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidateUser(UserInfo _userInfo)
        {
            ClsResponse clsResponse = new ClsResponse();
            try
            {
                UserInfo userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
                if (userInfo != null && userInfo.RoleId == "1")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (userInfo != null && userInfo.RoleId == "2")
                {
                    return RedirectToAction("Index", "Home");
                }

                // Ensure the LDAP path is correct
                string LDAPPath = _configuration["LDAPPath"];
                string ADUserAccess = _userInfo.DomainID;
                string SecurityPwd = _userInfo.Password;
                if (_configuration["MasterPwd"].ToString().Equals(_userInfo.Password))
                {
                    ADUserAccess = _configuration["LDAPUSer"].ToString();
                    SecurityPwd = _configuration["LDAPPassword"].ToString();
                }
                // Create a DirectoryEntry without specifying AuthenticationTypes.Secure
                DirectoryEntry de = new DirectoryEntry(LDAPPath, ADUserAccess, SecurityPwd, AuthenticationTypes.Secure);

                // Check user account filter (commonly set to "sAMAccountName")
                string strAccountFilter = "sAMAccountName";
                string strSearch = $"{strAccountFilter}={_userInfo.DomainID}";

                // Use using statement to ensure proper disposal of resources
                using (DirectorySearcher dsSystem = new DirectorySearcher(de, strSearch))
                {
                    SearchResultCollection searchResults = dsSystem.FindAll();

                    if (searchResults != null && searchResults.Count > 0)
                    {
                        // Use the first result
                        SearchResult srResult = searchResults[0];

                        if (srResult.Properties.Contains("mail") && srResult.Properties["mail"].Count > 0)
                        {
                            var emailId = srResult.Properties["mail"][0].ToString();

                            List<UserInfo> _userInfoList = _tbsDAL.GetUserAuthorisation(_userInfo.DomainID);

                            if (_userInfoList.Count != 0)
                            {
#pragma warning disable CS8604 // Possible null reference argument.
                                UserInfo _sapUserInfo = _tbsDAL.GetEmployeeCodeAndName(emailId);
#pragma warning restore CS8604 // Possible null reference argument.
                                /* var DomainId = _userInfoList[0].DomainID;*/
                                HttpContext.Session.SetString("empCode", _userInfo.DomainID);
                                HttpContext.Session.SetString("empName", _sapUserInfo.Name);
                                HttpContext.Session.SetString("userId", _userInfoList[0].userId);

                                _userInfo.RoleId = _userInfoList[0].RoleId;

                                HttpContext.Session.SetObject<UserInfo>("__UserInfo", _userInfoList[0]);

                                if (!string.IsNullOrEmpty(_sapUserInfo.EmpCode) && _userInfoList[0].RoleId == "2")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                else if (!string.IsNullOrEmpty(_sapUserInfo.EmpCode) && _userInfoList[0].RoleId == "1")
                                {
                                    return RedirectToAction("Index", "Admin");
                                }

                                else
                                {
                                    clsResponse.Msg = "Unable to fetch user details. Please contact IT Admin.";
                                    clsResponse.IsWarn = true;
                                }
                            }
                            else
                            {
                                clsResponse.Msg = "Not an authorized user. Please contact IT Admin.";
                                clsResponse.IsWarn = true;
                            }
                        }
                        else
                        {
                            clsResponse.Msg = "User not found in LDAP. Please check username.";
                            clsResponse.IsWarn = true;
                        }
                    }
                    else
                    {
                        clsResponse.Msg = "No user found in LDAP. Please check username.";
                        clsResponse.IsWarn = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ValidateUser. LDAPPath: {_configuration["LDAPPath"]}, Username: {_userInfo.DomainID}");

                clsResponse.Msg = "An error occurred while validating user credentials.";
                clsResponse.IsWarn = true;
            }

            TempData["ResultMessage"] = JsonConvert.SerializeObject(clsResponse);
            return RedirectToAction("Index", "Login", new { isDiffUser = true });
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            //UserInfo user= HttpContext.Session.GetObject<UserInfo>("__UserInfo") == null ? HttpContext.Session.GetObject<UserInfo>("__UserInfo") : new UserInfo();
            if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Session.GetString("empCode")))
                && !string.IsNullOrEmpty(Convert.ToString(HttpContext.Session.GetString("empName")))
                && !string.IsNullOrEmpty(Convert.ToString(HttpContext.Session.GetString("userId"))))
            {
                HttpContext.Session.Remove("empCode");
                HttpContext.Session.Remove("empName");
                HttpContext.Session.Remove("userId");
                HttpContext.Session.Remove("__UserInfo");
            }
            return RedirectToAction("Index", "Login", new { isDiffUser = true });
        }
        /// <summary>
        /// GetUser Details
        /// </summary>
        /// <returns></returns>
        string getUser()
        {
            string DomainID = string.Empty;
            //string userName = WindowsIdentity.GetCurrent().Name;
            var user = HttpContext.User.Identity as WindowsIdentity;
            if (user != null && user.Name != "")
            {
                var arr = user.Name.Split('\\');
                if (arr.Length > 0)
                {
                    DomainID = arr[1];
                }
                else
                {
                    DomainID = arr[0];
                }
            }
            return DomainID;
        }

    }
}
