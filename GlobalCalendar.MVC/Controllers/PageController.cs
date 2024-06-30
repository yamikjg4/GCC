using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using Microsoft.AspNetCore.Mvc;

namespace GlobalCalendar.MVC.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Index()
        {
            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
