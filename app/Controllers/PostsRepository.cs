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

    public class PostsRepository : IPostsRepository
    {
        // private readonly ILogger<PostsRepository> _logger;
        public ApplicationDbContext _context { get; }

        public PostsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // post because of validation token

        public async Task<IEnumerable<PostModel>> getPostsByRoomId(int id)
        {
            return await _context.Posts
                .Where(post => post.RoomId == id)
                .OrderByDescending(post => post.Id)
                .Take(100)
                .Include(post => post.Room)
                .ThenInclude(room => room.UsersLink)
                .Select(post => new PostModel { 
                    PostBody = post.PostBody, 
                    UserName = post.User.NickName, 
                    CreateDate = post.CreateDate,
                    RoomId = post.RoomId,
                    Identifier = post.Identifier.ToString()
                })
                .AsNoTracking()
                .ToListAsync();

        }

    }
    public interface IPostsRepository
    {
        Task<IEnumerable<PostModel>> getPostsByRoomId(int id);
    }
}
