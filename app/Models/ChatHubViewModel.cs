using System.Collections.Generic;
using app.Controllers;

namespace app.Models
{

    public class ChatHubViewModel
    {
        public IEnumerable<PostModel> Posts { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
    }

}