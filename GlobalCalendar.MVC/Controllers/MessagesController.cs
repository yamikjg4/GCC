using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Web.Data;
using Chat.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Chat.Web.Hubs;
using Chat.Web.ViewModels;
using System.Text.RegularExpressions;
using GlobalCalendar.MVC.Models;
using GlobalCalendar.MVC.SessionManagement;
using System.DirectoryServices.ActiveDirectory;
using GlobalCalendar.MVC.DAL;
using Chat.Web.Helpers;
using Microsoft.AspNetCore.Html;

namespace Chat.Web.Controllers
{
  /*  [Authorize]*/
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly CommonRepo repo;
        private readonly TBSDAL _tbsDAL;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
        public MessagesController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext,CommonRepo repo, TBSDAL tbsDAL,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
            this.repo = repo;
            _tbsDAL = tbsDAL;
            _webHostEnvironment= webHostEnvironment;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound();

            var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
            return Ok(messageViewModel);
        }

        [HttpGet("Room/{roomName}")]
        public IActionResult GetMessages(string roomName)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return BadRequest();

            UserInfo _userInfo = HttpContext.Session.GetObject<UserInfo>("__UserInfo");
            if (_userInfo == null)
                return BadRequest(); // You might want to handle this case appropriately

            string domainId = HttpContext.Session.GetString("empCode");
            if (_userInfo.RoleId == "1")
            {
                var messages = (from message in _context.Messages
                                join rooms in _context.Rooms on message.ToRoomId equals rooms.Id
                                join user in _context.Users_MST on message.FromUserId equals user.DomainId
                                where message.ToRoomId == room.Id
                                select new MessageViewModel
                                {
                                    Id = message.Id,
                                    Content = BasicEmojis.ParseEmojis(message.Content),
                                    Timestamp = message.Timestamp,
                                    FromUserName = user.DomainId,
                                    FromFullName = user.Employee_Name,
                                    Room = rooms.Name,
                                    Avatar = null
                                }
                                ).OrderByDescending(m => m.Timestamp)
                                .AsEnumerable()
                                .Reverse()
                                .ToList();
                var lst = _context.Tbl_BrodcastMesHistrory.Where(x => x.DomainId == domainId && x.RoomId == room.Id).ToList();
                foreach (var item in lst)
                {
                    item.IsRead=true;
                    _context.UpdateRange(item);
                   
                }
                _context.SaveChanges();
                return Ok(messages);
            }
            else
            {
                var messages = (from message in _context.Messages
                                join rooms in _context.Rooms on message.ToRoomId equals rooms.Id
                                join user in _context.Users_MST on message.FromUserId equals user.DomainId
                                where message.ToRoomId == room.Id && (user.DomainId == "KMAMTA" || user.DomainId == "SMISHRA" || user.DomainId == domainId) 
                                select new MessageViewModel
                                {
                                    Id = message.Id,
                                    Content = BasicEmojis.ParseEmojis(message.Content),
                                    Timestamp = message.Timestamp,
                                    FromUserName = user.DomainId,
                                    FromFullName = user.Employee_Name,
                                    Room = rooms.Name,
                                    Avatar = null
                                }
                                ).OrderByDescending(m => m.Timestamp)
                                .AsEnumerable()
                                .Reverse()
                                .ToList();
                var lst = _context.Tbl_BrodcastMesHistrory.Where(x => x.DomainId == domainId && x.RoomId==room.Id).ToList();
                foreach (var item in lst)
                {
                    item.IsRead = true;
                    _context.UpdateRange(item);
                   
                }
                _context.SaveChanges();
                return Ok(messages);
            }
        }


        [HttpPost]
        public async Task<ActionResult<Message>> Create(MessageViewModel viewModel)
        {
            string domainId = HttpContext.Session.GetString("empCode");
            var user = _tbsDAL.GetUserAuthorisation(domainId).FirstOrDefault();
            var room = _context.Rooms.FirstOrDefault(r => r.Name == viewModel.Room);
            if (room == null)
                return BadRequest();

            var msg = new Message()
            {
                Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
                FromUserId = user.EmpCode,
                ToRoomId = room.Id,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(msg);
            repo.InsertHistory(user.RoleId, room.Id);



            await _context.SaveChangesAsync();
            
              var model = new MessageViewModel()
              {
                  Id = msg.Id,
                  Content = BasicEmojis.ParseEmojis(msg.Content),
                  Timestamp = msg.Timestamp,
                  FromUserName = user.EmpCode,
                  FromFullName = user.Name,

                  Room = room.Name,
                  Avatar = null
              };
            
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", model);


            return CreatedAtAction(nameof(Get), new { id = msg.Id }, model);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string domainId = HttpContext.Session.GetString("empCode");
            var message = await (from u in _context.Messages
                                 join u2 in _context.Users_MST on u.FromUserId equals u2.DomainId
                                 where u.Id == id && u2.DomainId== domainId select u).FirstOrDefaultAsync();
            /*   var message = await _context.Messages
                   .Include(u => u.FromUser)
                   .Where(m => m.Id == id && m.FromUser.UserName == User.Identity.Name)
                   .FirstOrDefaultAsync();*/
           
            if (message == null)
                return NotFound();
            repo.DelleteBroadcasthistory(id);
          /*  _context.Messages.Remove(message);*/
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);

            return Ok();
        }
    }
}
