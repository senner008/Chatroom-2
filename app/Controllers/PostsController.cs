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
        public MessageHandler _messageHandler { get; }

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, MessageHandler messageHandler)
        {
            _context = context;
            _userManager = userManager;
            _messageHandler = messageHandler;
        }

        // post because of validation token
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> getPostsByRoomId(int id)
        {
            var headers = Request.Headers;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // TODO : create stored procedure :

            // SELECT `t`.`PostBody`, `a`.`NickName`, `t`.`CreateDate`, `r`.`IsPublic`, `t`.`Id`, `a`.`Id`, `r`.`Id`, `u`.`UserId`, `u`.`RoomId`
            // FROM(
            //     SELECT `p`.`Id`, `p`.`CreateDate`, `p`.`Likes`, `p`.`PostBody`, `p`.`RoomId`, `p`.`UpdateDate`, `p`.`UserId`
            //     FROM `Posts` AS `p`
            //     WHERE(`p`.`RoomId` = 1) AND 1 IS NOT NULL
            //     ORDER BY `p`.`Id` DESC
            //     LIMIT 100
            // ) AS `t`
            // INNER JOIN `AspNetUsers` AS `a` ON `t`.`UserId` = `a`.`Id`
            // INNER JOIN `Rooms` AS `r` ON `t`.`RoomId` = `r`.`Id`
            // LEFT JOIN `UserRoom` AS `u` ON `r`.`Id` = `u`.`RoomId`
            // ORDER BY `t`.`Id` DESC, `a`.`Id`, `r`.`Id`, `u`.`UserId`, `u`.`RoomId`;

            var posts = await _context.Posts

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

            watch.Stop();
            // // TODO : Why is the ef core sql execution twice as slow? 


            var elapsedMs = watch.ElapsedMilliseconds;
            System.Console.WriteLine("---------------------------");
            System.Console.WriteLine(elapsedMs);


            if (posts.Any())
            {
                try {
                    Room hasRoomAccess = await _messageHandler.FindAndValidateRoom(id);
                    System.Console.WriteLine("roomid:" + id);
                } catch (Exception ex) {
                    System.Console.WriteLine("dsffdsf");
                    return BadRequest("User room denied");
                }
            }

            return Ok(posts);

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
