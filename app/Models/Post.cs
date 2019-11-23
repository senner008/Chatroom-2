using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace app.Models
{

    public class Post : PostAbstract
    {

    }

    public class PostArchive : PostAbstract
    {

    }

    public class PostAbstract
    {
        public int Id { get; set; }

        [Required]
        public string PostBody { get; set; }

        public int Likes { get; set; }

        [Required]
        [Display(Name = "Create Date")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Update Date")]
        [DataType(DataType.Date)]
        public DateTime UpdateDate { get; set; }

        // navigation properties
        [Required]
        public string UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid Identifier { get; set; }

        public ApplicationUser User { get; set; }

        // TODO : rename Rooms to Room
        public Room Room { get; set; }
    }
}