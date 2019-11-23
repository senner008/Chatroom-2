using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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

         public async Task<string> GetCurrentUserNickName(string userId)
        {
            var nickname =  _hubLogger._connections.GetConnections(userId)?.NickName;
            if (nickname == null) {    
                try {
                    System.Console.WriteLine("lookup nickname...");
                    var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
                    nickname = user.NickName;
                    _hubLogger._connections.Update(userId, nickname); 
                } catch (Exception ex){
                    throw new Exception();
                }   
            }
            return nickname;  
        }

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
            var modelStateValidator = new ModelStateValidator();
            var validatePostMessage = modelStateValidator.ValidatePost<Postmessage>(postMessage);
            if (!validatePostMessage) throw new MyChatHubException(modelStateValidator.validationResults.FirstOrDefault().ErrorMessage);

            var post = CreatePost(postMessage.Message, postMessage.RoomId, userId);

            bool isValid = modelStateValidator.ValidatePost<Post>(post);
            if (!isValid) throw new MyChatHubException(modelStateValidator.validationResults.FirstOrDefault().ErrorMessage);

            return post;

        }

        public async Task<Room> FindAndValidateRoom(int roomId, string userId = null) 
        {
          
             var room = await FindRoom(roomId);
             if (room == null) throw new MyChatHubException("Invalid room selection");

             bool hasAccess = UserHasRoomAccess(room, userId);
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

        private bool UserHasRoomAccess(Room room, string userId = null)
        {
      
                        
                if (room.IsPublic) {
                   return true;
                }

                if (userId == null) {
                   userId = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }          

                var RoomHasUser = room.UsersLink.FirstOrDefault(userRoom => userRoom.UserId == userId);
                if (RoomHasUser != null) {
                    return true;   
                }
              

               return false;  
        }
      
        private Post CreatePost(string message, int roomId, string userId)
        {
           
            return new Post 
            { 
                UserId = userId, 
                RoomId = roomId, 
                PostBody = message, 
                CreateDate = DateTime.Now,
                Identifier = Guid.NewGuid()
            };
        }

    
    }
}