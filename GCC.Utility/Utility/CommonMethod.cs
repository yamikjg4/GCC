using ClosedXML.Excel;
using OfficeOpenXml;
using System.Data;

namespace GCC.Utility.Utility
{
    public static class CommonMethod
    {
        public static DataTable ConvertToDataTable<T>(List<T> data)
        {
            DataTable dataTable = new DataTable();

            // Check if the list is not empty
            if (data.Count > 0)
            {
                // Get the type of the objects in the list
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Type entityType = data[0].GetType();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                // Get the properties of the type
                var properties = entityType.GetProperties();

                // Create columns in the DataTable based on the properties of the object
                foreach (var property in properties)
                {
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

                // Populate the DataTable with data from the list
                foreach (var item in data)
                {
                    DataRow row = dataTable.NewRow();

                    foreach (var property in properties)
                    {
                        row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
        public static MemoryStream ExportToExcel(DataTable table)
        {
            // Create a new workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Insert DataTable into the worksheet
                worksheet.Cell(1, 1).InsertTable(table);

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save the Excel file to a MemoryStream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return stream;
            }
        }
        public static bool CheckExcelContent(Microsoft.AspNetCore.Http.IFormFile file)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context

                if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = file.OpenReadStream())
                    using (var package = new ExcelPackage(stream))
                    {
                        // Reset the position of the stream to the beginning
                        stream.Position = 0;

                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        if (worksheet != null)
                        {
                            // You can perform further checks on the worksheet if needed
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Worksheet is null. Check if the sheet exists and has data.");
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid file format. Expected .xlsx.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Exception details: {ex.StackTrace}");
                return false;
            }
        }
    }
}
