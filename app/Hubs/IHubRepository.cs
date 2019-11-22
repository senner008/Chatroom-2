using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;

namespace app.Repositories
{
    public interface IHubRepository 
    {
        IHubLogger _hubLogger { get; }
        Task<string> GetCurrentUserNickName(string id);
        Task SavePost(Post post);

        Post CreateAndValidatePost(Postmessage postMessage, string userId);
        Task<Room> FindAndValidateRoom(int roomId, string userId = null);
        IEnumerable<string> FindReceivers(Room room);
    }
}