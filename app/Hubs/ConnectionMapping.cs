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
     
        public  Dictionary<T, UserConnectionInfo> _connections =
            new Dictionary<T, UserConnectionInfo>();
        private UserAddedEvent userAdded;
        
        //https://stackoverflow.com/questions/2205711/should-i-lock-event
        private readonly object eventLock = new object();

        public event UserAddedEvent UserAdded
        {
            add
            {   
          
                lock (eventLock) 
                {
                        if (userAdded == null || !userAdded.GetInvocationList().Contains(value))
                        {
                            
                            System.Console.WriteLine("adding...");
                            userAdded += value;
                        }
                }
     
            }
            remove
            {
                lock (eventLock)
                {
                    System.Console.WriteLine("removing...");
                     userAdded -= value;
                }
                
            }
        }

        public void Add(T key, UserConnectionInfo userInfo)
        {
            lock (_connections)
            {
                if (!_connections.ContainsKey(key))
                {
                    _connections.Add(key, userInfo);
                }
                
            }

            UserAddedEvent handler;
            lock (eventLock)
            {
                handler = userAdded;
            }
                
            if (handler != null)
                userAdded(this, new AddMyUserEventArgs { Id = key as string});
        }

        public UserConnectionInfo GetConnections(T key)
        {
            lock (_connections)
            {
                if (_connections.ContainsKey(key))
                {
                    return _connections[key];
                }
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