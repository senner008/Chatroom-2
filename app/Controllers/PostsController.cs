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
using System.Threading;

namespace app.Controllers
{
    [Authorize (Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : Controller
    {
        public IHubRepository _hubRepository { get; }
        public IPostsRepository _postsRepository { get; }

        public PostsController(IHubRepository hubRepository, IPostsRepository postsRepository)
        {
            _hubRepository = hubRepository;
            _postsRepository = postsRepository;
        }

        // post because of validation token
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> getPostsByRoomId(int id)
        {
            IEnumerable<PostModel> posts;
            try {
                Room hasRoomAccess = await _hubRepository.FindAndValidateRoom(id);

                posts = await _postsRepository.getPostsByRoomId(id);

            } catch (MyChatHubException ex) {
                 return BadRequest(ex.Message);
            } catch (Exception ex) {
                throw new Exception(); 
            } 

            if (posts != null && posts.Any())
            {
                // Response.Headers.Add("Response-message", "Posts received for room " + id);
                return Ok(posts);
            }

            return NotFound("There are no posts in this room");

        }

    }
 
}
