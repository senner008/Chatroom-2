using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace app.Models
{

 public class UserRoom
    {
      [Required]
      public string UserId { get; set; }
      
      [Required]
      public int RoomId { get; set; }

      public ApplicationUser User { get; set; }
      public Room Room { get; set; }

    }
}