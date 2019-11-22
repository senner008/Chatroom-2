using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using app.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using app.Repositories;
using System.Net.Http;
using System.Net;

namespace app.Controllers
{
    [Authorize (Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : Controller
    {
        // private readonly ILogger<PostsController> _logger;
        public ApplicationDbContext _context { get; }
        public UserManager<ApplicationUser> _userManager { get; }
        public IHubRepository _hubRepository { get; }

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubRepository hubRepository)
        {
            _context = context;
            _userManager = userManager;
            _hubRepository = hubRepository;
        }

        // post because of validation token
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> getPostsByRoomId(int id)
        {
            // throw new Exception();
            List<PostModel> posts;
            try {
                Room hasRoomAccess = await _hubRepository.FindAndValidateRoom(id);

                posts = await _context.Posts
                    .Where(post => post.RoomId == id)
                    .OrderByDescending(post => post.Id)
                    .Take(10)
                    .Include(post => post.Rooms)
                    .ThenInclude(room => room.UsersLink)
                    .Select(post => new PostModel { 
                        PostBody = post.PostBody, 
                        UserName = post.User.NickName, 
                        CreateDate = Helper.ToMiliseconds(post.CreateDate),
                        RoomId = post.RoomId
                    })
                    .AsNoTracking()
                    .ToListAsync();

            } catch (MyChatHubException ex) {
                 return BadRequest(ex.Message);
            } catch (Exception ex) {
                return BadRequest("An error has occurred");
            } 

            if (posts != null && posts.Any())
            {
                Response.Headers.Add("Response-message", "Posts received");
                return Ok(posts);
            }

            return NotFound("Unable to retrieve posts");

        }

    }
    public class PostModel
    {
        public string PostBody { get; set; }

        public string UserName { get; set; }

        public double CreateDate { get; set; }

        public int RoomId { get; set;}

    }
}
