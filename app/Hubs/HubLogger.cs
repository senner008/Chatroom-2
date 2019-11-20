using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class HubLogger : IHubLogger
    {
        public ConnectionMapping<string> _connections { get; }

        public HubLogger()
        {
            _connections = new ConnectionMapping<string>();
            System.Console.WriteLine("hub logge constructed");
        }
    }

    public interface IHubLogger
    {
        ConnectionMapping<string> _connections { get; }
    }

}