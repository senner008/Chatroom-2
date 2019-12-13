using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app;
using app.Models;
using app.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    [Authorize (Roles = "Admin")]
    public class ChatHub : Hub
    {
        
        public IHubRepository _hubRepository { get; }
        public IHubLogger _hubLogger { get; }

        public ChatHub(IHubRepository hubRepository, IHubLogger hubLogger)
        {            
            _hubRepository = hubRepository;
            _hubLogger = hubLogger;
        }
        private List<string> receivers = null;
        private  Post post = null;
        private Room room = null;

        private string nickname = null;

        // This event will send message to users who connect between message sent and add to db ( in danger zone )
        private async Task SendToUserAdded (string userAddedId) {
            string findInReceievers = receivers.FirstOrDefault (id => id == userAddedId);
            if (room.IsPublic || findInReceievers != null) {
                await SendWSMessage (Clients.User (userAddedId));
               System.Console.WriteLine ("User: " + userAddedId + " has logged on and received message: " + post.PostBody);
            }
        }
        private async Task SendWSMessage (IClientProxy proxy) {
        // Folowing operation here can cause InvalidOperationException : 
        // await _hubRepository.GetCurrentUserNickName(Context.UserIdentifier);
        // An exception occurred while iterating over the results of a query for context type 'app.Data.ApplicationDbContext'.
        // System.InvalidOperationException: A second operation started on this context before a previous operation completed. This is usually caused by different threads using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.
            await proxy.SendAsync (
                "ReceiveMessage", 
                nickname, 
                post.PostBody, 
                post.RoomId, 
                post.CreateDate, 
                post.Identifier.ToString()
            );
        }
        private async void UserAddedCallback (object sender, AddMyUserEventArgs args) => await SendToUserAdded (args.Id);
        
        public async Task SendMessage ([FromBody] Postmessage postmessage) {
  
            try {
                nickname = await _hubRepository.GetCurrentUserNickName(Context.UserIdentifier);
                room = await _hubRepository.FindAndValidateRoom (postmessage.RoomId, Context.UserIdentifier);
                post = _hubRepository.CreateAndValidatePost (postmessage, Context.UserIdentifier);
                receivers = _hubRepository.FindReceivers (room).ToList ();

                // Subscribe to event
                _hubLogger._connections.UserAdded += UserAddedCallback;

                // Send Websocket message
                await SendWSMessage (room.IsPublic ? Clients.All : Clients.Users (receivers));
     
                // Message saved
                await _hubRepository.SavePost (post);

            } catch (MyChatHubException ex) {
                // sendAsync exceptions are also caught here
                // create custom IHubRepository exception
                // create generoic message for all other exceptions
                await Clients.Caller.SendAsync ("ErrorMessage", ex.Message);
            } catch (Exception ex) {
               await Clients.Caller.SendAsync ("ErrorMessage", "FatalError");        
            } finally {
                // System.Console.WriteLine ("FINALLY!");
                // UNSUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded -= UserAddedCallback;
            }
 
        }

        public override async Task OnConnectedAsync()
	    {

            System.Console.WriteLine(Context.ConnectionId);
            var id = Context.UserIdentifier;
             _hubLogger._connections.Add(id, new UserConnectionInfo { ConnectionId = Context.ConnectionId});

            // System.Console.WriteLine("----------------");
            // System.Console.WriteLine("User has connected : ");
	        // System.Console.WriteLine(Context.User.Identity.Name);
            // System.Console.WriteLine(id);
            // System.Console.WriteLine("----------------");

            await base.OnConnectedAsync();
	    }

        public override async Task OnDisconnectedAsync(Exception exception)
	    {       

            var id = Context.UserIdentifier;
            _hubLogger._connections.Remove(id);

            // System.Console.WriteLine("----------------");
            // System.Console.WriteLine("User has dis-connected : ");
	        // System.Console.WriteLine(Context.User.Identity.Name);
            // System.Console.WriteLine(id);
            // System.Console.WriteLine("----------------");

            await base.OnDisconnectedAsync(exception);
	    }

    }

}