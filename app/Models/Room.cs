using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsPublic { get; set; }
        public ICollection<UserRoom> UsersLink { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}