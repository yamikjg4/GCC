using System.Data;

namespace GlobalCalendar.MVC.Utility
{
    public static class CommonFuction
    {
        public static List<T> ConvertDataTableToList<T>(DataTable dataTable)
        {
            List<T> dataList = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T dataItem = Activator.CreateInstance<T>();

                foreach (DataColumn column in dataTable.Columns)
                {
                    // Use reflection to set property values
                    typeof(T).GetProperty(column.ColumnName)?.SetValue(dataItem, row[column]);
                }

                dataList.Add(dataItem);
            }

            return dataList;
        }
        

    }
}
