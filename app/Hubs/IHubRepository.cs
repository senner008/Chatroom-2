using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Models;
using SignalRChat.Hubs;

namespace app.Repositories
{
    public interface IHubRepository 
    {
        void AddUserToDictionary(string userId, string connectionId);
        void RemoveUserFromDictionary(string id);

        IHubLogger _hubLogger { get; }

        Task<string> GetCurrentUserNickName(string id);
        Task SavePost(Post post);
        Post CreateAndValidatePost(Postmessage postMessage, string userId);
        Task<Room> FindAndValidateRoom(int roomId, string userId = null);
        IEnumerable<string> FindReceivers(Room room);
    }
}