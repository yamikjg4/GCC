using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Web.Data;
using Chat.Web.Models;
using Microsoft.AspNetCore.Authorization;
/*using AutoMapper;*/
using Chat.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Chat.Web.ViewModels;
using System.DirectoryServices.ActiveDirectory;
/*using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;*/
using dotless.Core.Parser.Tree;
using GlobalCalendar.MVC.Services;

namespace Chat.Web.Controllers
{
/*    [Authorize]*/
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
      
        public RoomsController(ApplicationDbContext context,
            IHubContext<ChatHub> hubContext
           )
        {
            _context = context;
            _hubContext = hubContext;
          

        }
        public int GetRoomCount(int RoomId)
        {
            string domainId = HttpContext.Session.GetString("empCode");
            var roomCounts =  _context.Tbl_BrodcastMesHistrory
                .Where(x => x.DomainId == domainId && !x.IsRead && x.RoomId==RoomId)
                .GroupBy(x => x.RoomId)
                .Count();
            return roomCounts;
          
        }
    /*    [HttpGet("SendEmail")] // Specify the route for this action
        public async Task<IActionResult> SendEmail()
        {
            await _emailSenderService.ExecuteAsync();
            return Ok();
        }*/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {
            string domainId = HttpContext.Session.GetString("empCode");

            // Query to get room counts


            // Query to get rooms
            var rooms = await(from r in _context.Rooms
                              join u in _context.Users_MST on r.AdminId equals u.DomainId
                              join b in (from b1 in _context.Tbl_BrodcastMesHistrory
                                         where b1.DomainId == domainId && b1.IsRead == false
                                         select b1) on r.Id equals b.RoomId into bGroup
                              from b in bGroup.DefaultIfEmpty()
                              group b by new { r.Id, r.Name, u.DomainId } into g
                              select new
                              {
                                  Id = g.Key.Id,
                                  Name = g.Key.Name,
                                  Admin = g.Key.DomainId,
                                  cnt = g.Select(x => x.Id).Where(x => x != null).Distinct().Count() // Count the number of unread messages in the group
                              }).ToListAsync();


            /*   var rooms = await _context.Rooms
                   .Include(r => r.Admin)
                   .ToListAsync();*/

            /*  var roomsViewModel = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomViewModel>>(rooms);*/

            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var room = await (from r in _context.Rooms
                              join u in _context.Users_MST on r.AdminId equals u.DomainId
                              where r.Id ==id
                              select new RoomViewModel()
                              {
                                  Id = r.Id,
                                  Name = r.Name,
                                  Admin = u.DomainId
                              }).FirstOrDefaultAsync();
            if (room == null)
                return NotFound();

         /*   var roomViewModel = _mapper.Map<Room, RoomViewModel>(room);*/
            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Create(RoomViewModel viewModel)
        {
            if (_context.Rooms.Any(r => r.Name == viewModel.Name))
                return BadRequest("Invalid room name or room already exists");
            string domainId = HttpContext.Session.GetString("empCode");
          
            var user = _context.Users_MST.FirstOrDefault(u => u.DomainId == domainId);
            var room = new Room()
            {
                Name = viewModel.Name,
                AdminId = user.DomainId
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            var model = new RoomViewModel()
            {
                Id = room.Id,
                Name = viewModel.Name,
                Admin = room.AdminId
            };
           
            await _hubContext.Clients.All.SendAsync("addChatRoom", model);

            return CreatedAtAction(nameof(Get), new { id = room.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, RoomViewModel viewModel)
        {
            if (_context.Rooms.Any(r => r.Name == viewModel.Name))
                return BadRequest("Invalid room name or room already exists");
            string domainId = HttpContext.Session.GetString("empCode");
            var room = await (from r in _context.Rooms
                              join u in _context.Users_MST on r.AdminId equals u.DomainId
                              where u.DomainId == domainId && u.DomainId != null
                              select new RoomViewModel()
                              {
                                  Id = r.Id,
                                  Name = r.Name,
                                  Admin = u.DomainId
                              }).FirstOrDefaultAsync();

            /*  var room = await _context.Rooms
                  .Include(r => r.Admin)
                  .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                  .FirstOrDefaultAsync();*/

            if (room == null)
                return NotFound();

            room.Name = viewModel.Name;
            await _context.SaveChangesAsync();

            //var updatedRoom = _mapper.Map<Room, RoomViewModel>(room);
            await _hubContext.Clients.All.SendAsync("updateChatRoom", room);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
             string domainId = HttpContext.Session.GetString("empCode");
            var room = await (from r in _context.Rooms
                              join u in _context.Users_MST on r.AdminId equals u.DomainId
                              where u.DomainId == domainId && u.DomainId != null && r.Id == id
                              select r).FirstOrDefaultAsync();
            /*var room = await _context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.Admin.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();*/

            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted");

            return Ok();
        }
    }
}
