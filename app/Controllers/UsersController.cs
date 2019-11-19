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

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersControler : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersControler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        
        [HttpPost]
        [Route("/Users")]
        public async Task<IActionResult> Index()
        {
            List<UserModel> users;
            try {
                users = await _userManager.Users.Select(user => new UserModel { NickName = user.NickName }).ToListAsync();
            } catch (Exception ex) {
                return BadRequest("Users could not be retrieved");
            }
            
            return Ok(users);
        }

    }

    class UserModel
    {
        public string NickName { get; set; }
    }

}
