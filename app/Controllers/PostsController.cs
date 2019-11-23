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
using System.Numerics;

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
        public IPostsRepository _postsRepository { get; }

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubRepository hubRepository, IPostsRepository postsRepository)
        {
            _context = context;
            _userManager = userManager;
            _hubRepository = hubRepository;
            _postsRepository = postsRepository;
        }

        // post because of validation token
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> getPostsByRoomId(int id)
        {
            // throw new Exception();
            IEnumerable<PostModel> posts;
            try {
                Room hasRoomAccess = await _hubRepository.FindAndValidateRoom(id);

                posts = await _postsRepository.getPostsByRoomId(id);

            } catch (MyChatHubException ex) {
                 return BadRequest(ex.Message);
            } catch (Exception ex) {
                return BadRequest("An error has occurred");
            } 

            if (posts != null && posts.Any())
            {
                Response.Headers.Add("Response-message", "Posts received for room " + id);
                return Ok(posts);
            }

            return NotFound("Unable to retrieve posts");

        }

    }
    public class PostModel
    {
        public string PostBody { get; set; }

        public string UserName { get; set; }

        public DateTime CreateDate { get; set; }

        public int RoomId { get; set;}

        public string Identifier { get; set; }

    }
}
