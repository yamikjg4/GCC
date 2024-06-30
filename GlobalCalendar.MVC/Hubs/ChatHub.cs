using AutoMapper;
using Chat.Web.Data;
using Chat.Web.Models;
using Chat.Web.ViewModels;
using GlobalCalendar.MVC.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat.Web.Hubs
{
/*    [Authorize]*/
    public class ChatHub : Hub
    {
        private readonly static List<UserViewModel> _Connections = new List<UserViewModel>();
        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();
        private readonly TBSDAL _tbsDAL;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpcontext;

        public ChatHub(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpcontext, TBSDAL tbsDAL)
        {
            _context = context;
            _mapper = mapper;
            _httpcontext = httpcontext;
            _tbsDAL = tbsDAL;
        }

        public async Task SendPrivate(string receiverName, string message)
        {
            if (_ConnectionsMap.TryGetValue(receiverName, out string userId))
            {
                var UserInfo = _tbsDAL.GetUserAuthorisation(IdentityName).FirstOrDefault();
                var sender = _Connections.FirstOrDefault(u => u.UserName == IdentityName);

                if (sender != null && !string.IsNullOrWhiteSpace(message))
                {
                    var messageViewModel = new MessageViewModel
                    {
                        Content = Regex.Replace(message, @"<.*?>", string.Empty),
                        FromUserName = sender.UserName,
                        FromFullName = sender.FullName,
                        Avatar = sender.Avatar,
                        Room = "",
                        Timestamp = DateTime.Now
                    };

                    await Clients.Client(userId).SendAsync("newMessage", messageViewModel);
                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
                }
            }
        }

        public async Task Join(string roomName)
        {
            try
            {
                var UserInfo = _tbsDAL.GetUserAuthorisation(IdentityName).FirstOrDefault(); ;
                var user = _Connections.FirstOrDefault(u => u.UserName == IdentityName);
                if (user != null && user.CurrentRoom != roomName)
                {
                    if (!string.IsNullOrEmpty(user.CurrentRoom))
                        await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

                    await Leave(user.CurrentRoom);
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                    user.CurrentRoom = roomName;

                    await Clients.OthersInGroup(roomName).SendAsync("addUser", user);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
            }
        }

        public async Task Leave(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public IEnumerable<UserViewModel> GetUsers(string roomName)
        {
            return _Connections.Where(u => u.CurrentRoom == roomName).ToList();
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var UserInfo = _tbsDAL.GetUserAuthorisation(IdentityName).FirstOrDefault();
                //var user = _context.Users_MST.FirstOrDefault(u => u.DomainId == IdentityName);
                var userViewModel = new UserViewModel
                {
                    Device = GetDevice(),
                    CurrentRoom = "",
                    FullName = UserInfo.Name,
                    UserName = UserInfo.EmpCode,
                    Avatar = null
                };
                if ( !_Connections.Any(u => u.UserName == IdentityName))
                {
                   

                    _Connections.Add(userViewModel);
                    _ConnectionsMap.Add(userViewModel.UserName, Context.ConnectionId);

                 
                }
                await Clients.Caller.SendAsync("getProfileInfo", userViewModel);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var UserInfo = _tbsDAL.GetUserAuthorisation(IdentityName).FirstOrDefault();
                var user = _Connections.FirstOrDefault(u => u.UserName == IdentityName);
                if (user != null)
                {
                    _Connections.Remove(user);
                    await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);
                    _ConnectionsMap.Remove(user.UserName);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }
            await base.OnDisconnectedAsync(exception);
        }
        private string IdentityName
        {
            get { string DomainId = !string.IsNullOrEmpty( _httpcontext.HttpContext.Session.GetString("empCode"))? _httpcontext.HttpContext.Session.GetString("empCode"):string.Empty;
                return DomainId;
            }
        }
        private string GetDevice()
        {
            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
            return (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile"))) ? device : "Web";
        }
    }
}
