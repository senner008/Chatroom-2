using System;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

     
        private readonly UserManager<ApplicationUser> _userManager;
        public HubLogger _hubLogger { get; }

        public ChatHub(UserManager<ApplicationUser> userManager, HubLogger hubLogger)
        {
            _userManager = userManager;
            _hubLogger = hubLogger;
        }

        public override async Task OnConnectedAsync()
	    {
            System.Console.WriteLine("----------------");
            System.Console.WriteLine("User has connected : ");
	        System.Console.WriteLine(Context.User.Identity.Name);
            var id = (await _userManager.FindByNameAsync(Context.User.Identity.Name)).Id;
	        System.Console.WriteLine(id);
            System.Console.WriteLine("----------------");
     
            _hubLogger._connections.Add(id, Context.ConnectionId);

            await base.OnConnectedAsync();

	    }

          public override async Task OnDisconnectedAsync(Exception exception)
	    {
            System.Console.WriteLine("----------------");
            System.Console.WriteLine("User has DISConnected : ");
	        System.Console.WriteLine(Context.User.Identity.Name);
            var id = (await _userManager.FindByNameAsync(Context.User.Identity.Name)).Id;
            System.Console.WriteLine(id);
            System.Console.WriteLine("----------------");

            _hubLogger._connections.Remove(id, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
	    }

    }

}