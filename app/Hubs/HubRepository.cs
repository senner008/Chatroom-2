using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalRChat.Hubs;

namespace app.Repositories
{
    public class  HubRepository : IHubRepository 
    {
        public ApplicationDbContext _context { get; }
        public UserManager<ApplicationUser> _userManager { get; }
        public IHttpContextAccessor _httpContext { get; }
        public IHubLogger _hubLogger { get; }
     
        public HubRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext, IHubLogger hubLogger)
        {

            _context = context;
            _userManager = userManager;
            _httpContext = httpContext;
            _hubLogger = hubLogger;
        }

        public void AddUserToDictionary(string userId, string connectionId) 
        {
            _hubLogger._connections.Add(userId, new UserConnectionInfo { ConnectionId = connectionId});
        }

       
        public void RemoveUserFromDictionary(string id) 
        {
            _hubLogger._connections.Remove(id);
        }

         public async Task<string> GetCurrentUserNickName(string userId)
        {
            var nickname =  _hubLogger._connections.GetConnections(userId)?.NickName;
            if (nickname == null) {
                System.Console.WriteLine("getting nickname...");
                nickname =  (await _userManager.GetUserAsync(_httpContext.HttpContext.User)).NickName;
                _hubLogger._connections.Update(userId, nickname);
            }
             return nickname;  
        }

        


        // public void AttachUserAddedEvent(Action<object,AddMyUserEventArgs> event2)
        // {
        //     _hubLogger._connections.UserAdded += event2 as UserAddedEvent;
        // }

        // public void UnAttachUserAddedEvent(Delegate UserAddedEvent)
        // {
        //     throw new NotImplementedException();
        // }


        public async Task SavePost(Post post) 
        {
            try {
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException err) {
                // TODO : create custom exception
                throw new MyChatHubException("Unable to persist message");
            }
        }

        public Post CreateAndValidatePost(Postmessage postMessage, string userId) 
        {
            // var user =  await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            // if (user == null) throw new InvalidOperationException("Unable to retrieve user");
            
            var post = CreatePost(postMessage.Message, postMessage.RoomId, userId);

            bool isValid = ValidatePost(post);
            if (!isValid) throw new MyChatHubException("Invalid post submission");

            return post;

        }

        public async Task<Room> FindAndValidateRoom(int roomId, string userId = null) 
        {
             var room = await FindRoom(roomId);
             if (room == null) throw new MyChatHubException("Invalid room selection");

             bool hasAccess = await UserHasRoomAccess(room, userId);
             if (!hasAccess) throw new MyChatHubException("Room access denied");

             return room;

        }

        public IEnumerable<string> FindReceivers(Room room) 
        {
            return room.UsersLink.Where(userRoom => userRoom.RoomId == room.Id).Select(userRoom => userRoom.UserId);
        }

        private async Task<Room> FindRoom(int roomId) 
        {
             return await _context.Rooms
            .Include(room => room.UsersLink)
            .FirstOrDefaultAsync(room => room.Id == roomId);
        }

        private async Task<bool> UserHasRoomAccess(Room room, string userId = null)
        {
      

                if (room.IsPublic) {
                   return true;
                }

                if (userId == null) {
                    userId = (await _userManager.GetUserAsync(_httpContext.HttpContext.User)).Id;
                }          

                var RoomHasUser = room.UsersLink.FirstOrDefault(userRoom => userRoom.UserId == userId);
                if (RoomHasUser != null) {
                    return true;   
                }
              

               return false;  
        }
        private bool ValidatePost(Post post)
        {
            var context = new ValidationContext(post, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(post, context, validationResults, true);
        }

        private Post CreatePost(string message, int roomId, string userId)
        {
            // throw new Exception();
            return new Post 
            { 
                UserId = userId, 
                RoomId = roomId, 
                PostBody = message, 
                CreateDate = DateTime.Now
            };
        }

    
    }
}