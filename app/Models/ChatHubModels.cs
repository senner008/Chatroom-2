using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using app.Controllers;

namespace app.Models
{

    public class ChatHubViewModel
    {
        public IEnumerable<PostModel> Posts { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
    }

    public class Postmessage {

        [Required]
        [StringLength(1000, ErrorMessage = "Message is too long")]
        [HTMLSanitizerError(ErrorMessage = "Invalid message")]
        public string Message { get; set; }

        [Required]
        public int RoomId { get; set; }
    }

}