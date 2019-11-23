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

        public RoomsRepository(ILogger<RoomsRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<Room>> getRooms()
        {
            return await _context.Rooms.ToListAsync();
        }
  
    }

    public interface IRoomsRepository
    {
        Task<IEnumerable<Room>> getRooms();
    }


}
