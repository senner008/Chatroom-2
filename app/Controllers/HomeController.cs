using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using app.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize (Roles = "Admin")]
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IPostsRepository _postsRepository { get; }
        public IRoomsRepository _roomsRepository { get; }

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext, IPostsRepository postsRepository, IRoomsRepository roomsRepository)
        {
            _logger = logger;
            _postsRepository = postsRepository;
            _roomsRepository = roomsRepository;
        }


        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/RoomInit/{id}")]
        public IActionResult RoomInit([FromRoute] int id)
        {
            // handle global error if id invalid
            ViewBag.RouteId = id;
            return View("Index");
        }

        [Route("/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}
