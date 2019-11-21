using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app;
using app.Controllers;
using app.Models;
using app.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{


    public class ChatHub : Hub
    {
    
        public HubLogger _hubLogger { get; }
        public IHubRepository _hubRepository { get; }

        public ChatHub(IHubRepository hubRepository,HubLogger hubLogger)
        {

            _hubLogger = hubLogger;
            _hubRepository = hubRepository;

        }
        
        public async Task SendMessage ([FromBody] Postmessage postmessage) {
            List<string> receivers = null;
            Post post = null;
            Room room = null;
            async void UserAddedEvent (object sender, AddMyUserEventArgs args) => await SendToUserAdded (args.Id);

            // This event will send message to users who connect between message sent and add to db ( in danger zone )
            async Task SendToUserAdded (string userAddedId) {
                string findInReceievers = receivers.FirstOrDefault (id => id.Contains (userAddedId));
                if (room.IsPublic || findInReceievers != null) {
                    await SendWSMessage (Clients.User (userAddedId));
                    System.Console.WriteLine ("User: " + userAddedId + " has logged on and received message: " + post.PostBody);
                }
            }

            async Task SendWSMessage (IClientProxy proxy) {
                await proxy.SendAsync ("ReceiveMessage", post.User.NickName, post.PostBody, post.RoomId, Helper.ToMiliseconds (post.CreateDate));
            }

            try {
                room = await _hubRepository.FindAndValidateRoom (postmessage.RoomId);
                post = await _hubRepository.CreateAndValidatePost (postmessage);
                receivers = _hubRepository.FindReceivers (room).ToList ();

                // SUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded += UserAddedEvent;

                await SendWSMessage (room.IsPublic ? Clients.All : Clients.Users (receivers));

                ///
                /// DANGER ZONE
                ///

                // Simulate slow Db save 
                // TODO : create test 
                //  Thread.Sleep (20000);
                await _hubRepository.SavePost (post);
                System.Console.WriteLine ("post saved");

            } catch (Exception ex) {
                // TODO : handle and send down errors
                 await Clients.Caller.SendAsync ("ErrorMessage", ex.Message);
            } finally {
                System.Console.WriteLine ("FINALLY!");
                // UNSUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded -= UserAddedEvent;
            }
        }

        public override async Task OnConnectedAsync()
	    {
            System.Console.WriteLine("----------------");
            System.Console.WriteLine("User has connected : ");
	        System.Console.WriteLine(Context.User.Identity.Name);


            var id = Context.UserIdentifier;
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
            var id = Context.UserIdentifier;
            System.Console.WriteLine(id);
            System.Console.WriteLine("----------------");

            _hubLogger._connections.Remove(id, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
	    }

    }

    public class HubMessage
    {
 
    }

}