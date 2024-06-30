using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/*using AutoMapper;*/
using Chat.Web.Data;
using Chat.Web.Helpers;
using Chat.Web.Hubs;
using Chat.Web.Models;
using Chat.Web.ViewModels;
using GlobalCalendar.MVC.DAL;
using GlobalCalendar.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace Chat.Web.Controllers
{
/*    [Authorize]*/
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowedExtensions;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileValidator _fileValidator;
        private readonly TBSDAL tBSDAL;
        private readonly CommonRepo repo;
        public UploadController(ApplicationDbContext context,
            IWebHostEnvironment environment,
            IHubContext<ChatHub> hubContext,
            IConfiguration configruation,
            IFileValidator fileValidator,
            TBSDAL tBSDAL,CommonRepo repo)
        {
            _context = context;
            _environment = environment;
            _hubContext = hubContext;
            _fileValidator = fileValidator;

            FileSizeLimit = configruation.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowedExtensions = configruation.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
            this.tBSDAL = tBSDAL;
            this.repo = repo;
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([FromForm] UploadViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModel.File))
                    return BadRequest("Validation failed!");

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModel.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "Chat");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.File.CopyToAsync(fileStream);
                }
                string domainId = HttpContext.Session.GetString("empCode");
                var user = tBSDAL.GetUserAuthorisation(domainId).FirstOrDefault();
                var room = _context.Rooms.Where(r => r.Id == viewModel.RoomId).FirstOrDefault();
                if (user == null || room == null)
                    return NotFound();

                string htmlImage = string.Format(
                   "<a href=\"/GlobalCalendar/Chat/{0}\" target=\"_blank\">" +
                   "<img src=\"/GlobalCalendar/Chat/{0}\" class=\"post-image\">" +
                   "</a>", fileName);


                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    Timestamp = DateTime.Now,
                    FromUserId = user.EmpCode,
                    ToRoomId = room.Id
                };

                await _context.Messages.AddAsync(message);
                repo.InsertHistory(user.RoleId, room.Id);
                await _context.SaveChangesAsync();

                // Send image-message to group
                var model = new MessageViewModel()
                {
                    Id = message.Id,
                    Content = message.Content,
                    Timestamp = message.Timestamp,
                    FromUserName = user.EmpCode,
                    FromFullName = user.Name,

                    Room = room.Name,
                    Avatar = null
                };
                await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", model);

                return Ok();
            }

            return BadRequest();
        }
    }
}
