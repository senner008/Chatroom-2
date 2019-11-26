using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using app.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize (Roles = "Admin")]
    public class RoomsRepository : IRoomsRepository
    {
        private readonly ILogger<RoomsRepository> _logger;
        public ApplicationDbContext _context { get; }
        public IHubContext<ChatHub> _hubContext { get; }
        public UserManager<ApplicationUser> _userManager { get; }

        public RoomsRepository(ILogger<RoomsRepository> logger, ApplicationDbContext context, IHubContext<ChatHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> getUsersByNuckname(RoomCreateModel roomCreateModel) 
        {
            return await _userManager.Users.Where(user => roomCreateModel.UserList.Any(nickname => user.NickName == nickname)).ToListAsync();
        }

        public async Task<List<Room>> getRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task addRoom(Room room) 
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task sendWSRoom(Room room)
        {
             await _hubContext.Clients.All.SendAsync ("CreateRoomMessage", room.Name, room.Id);
        }
  
    }

    public interface IRoomsRepository
    {
        Task<List<Room>> getRooms();
        Task addRoom(Room room);
        Task sendWSRoom(Room room);
        Task<List<ApplicationUser>> getUsersByNuckname(RoomCreateModel roomCreateModel);
    }


}
