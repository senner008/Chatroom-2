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
    public class ErrorController : Controller
    {
   
        public ErrorController()
        {
        }

        [Route("Error/{StatusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode) 
        {
            if (statusCode == 404) 
                ViewBag.ErrorMessage = "Sorry, the resource you requested not not exist";
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}
