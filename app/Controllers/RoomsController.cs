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
            List<Room> rooms;
            try {
                 rooms = await _roomsRepository.getRooms();
            }
            catch (Exception) {
                throw new Exception();
            }
           
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
    

            List<ApplicationUser> users;
            try {
                 users = await _userManager.Users.Where(user => roomCreateModel.UserList.Any(nickname => user.NickName == nickname)).ToListAsync();
            } catch (Exception) {
                throw new Exception(); 
            }

            List<Room> roomsList = new List<Room>();
            try {
                  roomsList = await _roomsRepository.getRooms();  
            } catch (Exception) {
                throw new Exception(); 
            }

            if (roomsList != null && roomsList.Any(room => room.Name == roomCreateModel.RoomName.Trim())) {
                return BadRequest("Room name already taken");
            }
         
            // TODO : in try catch
            List<UserRoom> userRoomsList = users.Select(user => new UserRoom {  UserId = user.Id}).ToList();
            var room = new Room
            {
                Name = roomCreateModel.RoomName,
                IsPublic = users.Count == 0 ? true : false,
                UsersLink = userRoomsList
            };
            try {
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync ("CreateRoomMessage", room.Name, room.Id);
            }
            catch (Exception) {
                throw new Exception();
            }
          
            return Ok();

        }
    }

   


}
