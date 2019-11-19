using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class HubLogger
    {
        public readonly  ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public HubLogger()
        {
            System.Console.WriteLine("hub logge constructed");
        }
    }

}