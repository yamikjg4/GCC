using GlobalCalendar.DAL;
using GlobalCalendar.MVC.Models;

namespace GlobalCalendar.MVC.DAL
{
    public class HolidayCalendar
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        GlobalCalendar.DAL.DBConnection objDb = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        /*     SAPDBConnection objSAPDb = null;*/
        public HolidayCalendar()
        {
            objDb = new DBConnection();
            /*  objSAPDb = new SAPDBConnection();*/
        }
        public List<HolidayCalendarModel> GetHolidayList()
        {
            List<HolidayCalendarModel> _HolidayLst = new List<HolidayCalendarModel>();
            var dt = objDb.ExecuteReader("[sp_checkLeave]");
            if (dt.Rows.Count > 0)
            {
                _HolidayLst = Utility.CommonFuction.ConvertDataTableToList<HolidayCalendarModel>(dt);
            }
            return _HolidayLst;
        }
    }
}
