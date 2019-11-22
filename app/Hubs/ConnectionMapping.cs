using System.Collections.Generic;
using System.Linq;

namespace SignalRChat.Hubs
{
    public class UserConnectionInfo
    {
        public string ConnectionId { get; set; }
        public string NickName { get; set; }
    }
    public delegate void UserAddedEvent(object sender, AddMyUserEventArgs args);

    public class AddMyUserEventArgs
    {
        public string Id { get; set; }
    }
    public class ConnectionMapping<T>
    {
     
        public readonly Dictionary<T, UserConnectionInfo> _connections =
            new Dictionary<T, UserConnectionInfo>();
        private UserAddedEvent userAdded;


        public event UserAddedEvent UserAdded
        {
            add
            {
                
                if (userAdded == null || !userAdded.GetInvocationList().Contains(value))
                {
                    System.Console.WriteLine("adding...");
                    userAdded += value;
                }
            }
            remove
            {
                System.Console.WriteLine("removing...");
                userAdded -= value;
            }
        }

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, UserConnectionInfo userInfo)
        {
            if (!_connections.ContainsKey(key))
                {
                    _connections.Add(key, userInfo);
                }

            if (userAdded != null)
                userAdded(this, new AddMyUserEventArgs { Id = key as string});
        }

        public UserConnectionInfo GetConnections(T key)
        {

            if (_connections.ContainsKey(key))
            {
                return _connections[key];
            }
            return null;

        }

        public void Update(T key, string nickName)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(key)) {
                    _connections[key] =  new UserConnectionInfo { ConnectionId = GetConnections(key).ConnectionId, NickName = nickName };
                }
            }
        }

        public void Remove(T key)
        {
            lock (_connections)
            {
                _connections.Remove(key);   
            }
        }
    }
}