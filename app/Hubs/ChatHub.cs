using System;
using System.Collections.Generic;
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
            System.Console.WriteLine("----------instantiate ChatHub-------------");
            
            _hubRepository = hubRepository;
            _hubLogger = hubLogger;
        }
        private List<string> receivers = null;
        private  Post post = null;
        private Room room = null;

        // This event will send message to users who connect between message sent and add to db ( in danger zone )
        private async Task SendToUserAdded (string userAddedId) {
            string findInReceievers = receivers.FirstOrDefault (id => id == userAddedId);
            if (room.IsPublic || findInReceievers != null) {
                await SendWSMessage (Clients.User (userAddedId));
                // System.Console.WriteLine ("User: " + userAddedId + " has logged on and received message: " + post.PostBody);
            }
        }
        private async Task SendWSMessage (IClientProxy proxy) {
            await proxy.SendAsync ("ReceiveMessage", await _hubRepository.GetCurrentUserNickName(Context.UserIdentifier), post.PostBody, post.RoomId, Helper.ToMiliseconds (post.CreateDate));
        }
        private async void UserAddedEvent (object sender, AddMyUserEventArgs args) => await SendToUserAdded (args.Id);
        
        public async Task SendMessage ([FromBody] Postmessage postmessage) {
  
            try {
                
                room = await _hubRepository.FindAndValidateRoom (postmessage.RoomId, Context.UserIdentifier);
                post = _hubRepository.CreateAndValidatePost (postmessage, Context.UserIdentifier);
                receivers = _hubRepository.FindReceivers (room).ToList ();

                // SUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded += UserAddedEvent;
                
                // _hubLogger._connections.UserAdded += UserAddedEvent;
                System.Console.WriteLine("sending message...");
                await SendWSMessage (room.IsPublic ? Clients.All : Clients.Users (receivers));

                ///
                /// DANGER ZONE
                ///

                // Simulate slow Db save 
                // TODO : create test 
                // Thread.Sleep (10000);
                await _hubRepository.SavePost (post);
                System.Console.WriteLine ("post saved");

            } catch (MyChatHubException ex) {
                // sendAsync exceptions are also caught here
                // create custom IHubRepository exception
                // create generoic message for all other exceptions
                await Clients.Caller.SendAsync ("ErrorMessage", ex.Message);
            } catch (Exception ex) {
                await Clients.Caller.SendAsync ("ErrorMessage", "An error has occurred");
                // throw new Exception(ex.Message);           
            } finally {
                // System.Console.WriteLine ("FINALLY!");
                // UNSUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded -= UserAddedEvent;
            }
 
        }

        public override async Task OnConnectedAsync()
	    {
        
            var id = Context.UserIdentifier;
            _hubRepository.AddUserToDictionary(id, Context.ConnectionId);

            System.Console.WriteLine("----------------");
            System.Console.WriteLine("User has connected : ");
	        System.Console.WriteLine(Context.User.Identity.Name);
            System.Console.WriteLine(id);
            System.Console.WriteLine("----------------");

            await base.OnConnectedAsync();
	    }

        public override async Task OnDisconnectedAsync(Exception exception)
	    {       

            var id = Context.UserIdentifier;
            _hubRepository.RemoveUserFromDictionary(id);

            System.Console.WriteLine("----------------");
            System.Console.WriteLine("User has dis-connected : ");
	        System.Console.WriteLine(Context.User.Identity.Name);
            System.Console.WriteLine(id);
            System.Console.WriteLine("----------------");

            await base.OnDisconnectedAsync(exception);
	    }

    }

    public class Postmessage {
        public string Message { get; set; }

        public int RoomId { get; set; }
    }

}