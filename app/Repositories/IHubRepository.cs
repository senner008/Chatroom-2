using System.Collections.Generic;
using System.Threading.Tasks;
using app.Controllers;
using app.Models;

namespace app.Repositories {
    public interface IHubRepository 
    {
        Task SavePost(Post post);
        Task<Post> CreateAndValidatePost(Postmessage postMessage);
        Task<Room> FindAndValidateRoom(int roomId);
        IEnumerable<string> FindReceivers(Room room);
    }
}