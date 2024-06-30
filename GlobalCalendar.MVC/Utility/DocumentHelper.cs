using Microsoft.AspNetCore.Hosting;
using System.Runtime.CompilerServices;

public static class DocumentHelper
{
    public static string filepath = "\\SELPUNNETSCH01\\Suzlon_Inhouse_File_Server_UAT\\GCC\\";
    public static async Task<string> UploadDocumentAsync(IFormFile file)
    {
        string fileName = "";
        if (file == null || file.Length == 0)
        {
            return fileName;
        }

        var uploadsFolder = Path.Combine(filepath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
    public static string UploadExcelAsync(IFormFile file)
    {
        string fileName = "";
        if (file == null || file.Length == 0)
        {
            return fileName;
        }

        var uploadsFolder = Path.Combine(filepath, "Excel");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
             file.CopyTo(stream);
        }

        return fileName;
    }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task<bool> DeleteFileasync(string FileName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        try
        {
            var filePath = Path.Combine(filepath, "uploads", FileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    

}
