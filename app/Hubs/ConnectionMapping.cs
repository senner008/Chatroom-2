using System.Collections.Generic;
using System.Linq;

namespace SignalRChat.Hubs
{
    public delegate void UserAddedEvent(object sender, AddMyUserEventArgs args);

    public class AddMyUserEventArgs
    {
        public string Id { get; set; }
    }
    public class ConnectionMapping<T>
    {
     
        public readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();
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

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
            if (userAdded != null)
                userAdded(this, new AddMyUserEventArgs { Id = key as string});
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}