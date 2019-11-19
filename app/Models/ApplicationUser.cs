using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace app.Models
{

 public class ApplicationUser : IdentityUser
    {
       public string NickName { get; set; }

       public ICollection<UserRoom> RoomsLink { get; set; }

       public ICollection<Post> Posts { get; set; }
    }
}