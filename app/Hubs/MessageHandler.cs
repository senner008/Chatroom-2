

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app.Controllers;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace app
{
    public class MessageHandler 
    {
        public ApplicationDbContext _context { get; }
        public UserManager<ApplicationUser> _userManager { get; }
        public IHttpContextAccessor _httpContext { get; }

        public MessageHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {

            _context = context;
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task SavePost(Post post) 
        {
            try {
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException err) {
                // TODO : create custom exception
                throw new InvalidOperationException("Unable to persist message");
            }
        
        }

        public async Task<Post> CreateAndValidatePost(Postmessage postMessage) 
        {
            var user =  await _userManager.GetUserAsync(_httpContext.HttpContext.User);
            if (user == null) throw new InvalidOperationException("Unable to retrieve user");
            
            var post = CreatePost(postMessage.Message, postMessage.RoomId, user);

            bool isValid = ValidatePost(post);
            if (!isValid) throw new InvalidOperationException("Invalid post submission");

            return post;

        }

        public async Task<Room> FindAndValidateRoom(int roomId) 
        {
             var room = await FindRoom(roomId);
             if (room == null) throw new InvalidOperationException("Invalid room selection");

             bool hasAccess = await UserHasRoomAccess(room);
             if (!hasAccess) throw new InvalidOperationException("Room access denied");

             return room;

        }

        public IEnumerable<string> FindReceivers(Room room) 
        {
            return room.UsersLink.Where(userRoom => userRoom.RoomId == room.Id).Select(userRoom => userRoom.UserId);
        }


        private async Task<Room> FindRoom(int roomId) 
        {
             return  await _context.Rooms
            .Include(room => room.UsersLink)
            .FirstOrDefaultAsync(room => room.Id == roomId);
        }

        private async Task<bool> UserHasRoomAccess(Room room)
        {
       
               if (room.IsPublic) {
                   return true;
               }

               var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);
               var RoomHasUser = room.UsersLink.FirstOrDefault(userRoom => userRoom.UserId == user.Id);

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

        private Post CreatePost(string message, int roomId, ApplicationUser user)
        {
            return new Post 
            { 
                UserId = user.Id, 
                RoomId = roomId, 
                PostBody = message, 
                CreateDate = DateTime.Now,
                User = user
            };
        }

    }
}