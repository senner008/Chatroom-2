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
using System.ComponentModel.DataAnnotations;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize (Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly ILogger<RoomsController> _logger;

        public IRoomsRepository _roomsRepository { get; }
        public ApplicationDbContext _context { get; }

        public UserManager<ApplicationUser> _userManager { get; }
        public IHubContext<ChatHub> _hubContext { get; }

        public RoomsController(IRoomsRepository roomsRepository, ILogger<RoomsController> logger, ApplicationDbContext context,  UserManager<ApplicationUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _roomsRepository = roomsRepository;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            // _context.Database.EnsureCreated();
        }

        [HttpPost]
        [Route("/Rooms")]
        public async Task<IActionResult> Index()
        {
            Response.Headers.Add("Response-message", "Rooms received");
            var rooms = await _roomsRepository.getRooms();
            return Ok(rooms.Select(room => new { Id =  room.Id, Name = room.Name }));
        }

        // [HttpGet]
        // [Route("/Rooms/UserRooms")]
        // public async Task<IActionResult> Userrooms()
        // {
        //     return Ok(await _context.Set<UserRoom>().ToListAsync());
        // }

        // TODO : Global validation fail handling
        [HttpPost]
        [Route("/Rooms/Create")]
        public async Task<IActionResult> Index([FromBody] RoomCreateModel roomCreateModel)
        {

            var getUsers = await _userManager.Users.Where(user => roomCreateModel.UserList.Any(nickname => user.NickName == nickname)).ToListAsync();
         
            // TODO : in try catch
            var room = new Room
            {
                Name = roomCreateModel.RoomName,
                IsPublic = getUsers.Count == 0 ? true : false,
                UsersLink = getUsers.Select(user => new UserRoom {  UserId = user.Id}).ToList()
            };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync ("CreateRoomMessage", room.Name, room.Id);
            
            return Ok();

        }
    }

   


}
