using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRChat.Hubs;

namespace app.Controllers {
    [ApiController]
    [Route ("[controller]")]
    [Authorize (Roles = "Admin")]

    public class HubController : ControllerBase {
        private readonly ILogger<HubController> _logger;

        public IHubContext<ChatHub> _hubContext { get; }
        public MessageHandler _messageHandler { get; }
        public HubLogger _hubLogger { get; }

        public HubController (ILogger<HubController> logger, IHubContext<ChatHub> hubContext, MessageHandler messageHandler, HubLogger hubLogger) {
            _logger = logger;
            _hubContext = hubContext;
            _messageHandler = messageHandler;
            _hubLogger = hubLogger;
        }

        // [Route ("createroom")]
        // [HttpPost]
        // public async Task<IActionResult> CreateRoom () {

        //     // TODO : only show rooms with access
        //     await _hubContext.Clients.All.SendAsync ("CreateRoomMessage", "room created");
        //     return Ok();
        // }

        [Route ("sendmessage")]
        [HttpPost]
        public async Task<IActionResult> SendMessage ([FromBody] Postmessage postmessage) {
            List<string> receivers = null;
            Post post = null;
            Room room = null;
            async void UserAddedEvent (object sender, AddMyUserEventArgs args) => await SendToUserAdded (args.Id);

            // This event will send message to users who connect between message sent and add to db ( in danger zone )
            async Task SendToUserAdded (string userAddedId) {
                string findInReceievers = receivers.FirstOrDefault (id => id.Contains (userAddedId));
                if (room.IsPublic || findInReceievers != null) {
                    await SendMessage (_hubContext.Clients.User (userAddedId));
                    System.Console.WriteLine ("User: " + userAddedId + " has logged on and received message: " + post.PostBody);
                }
            }

            async Task SendMessage (IClientProxy proxy) {
               
                await proxy.SendAsync ("ReceiveMessage", post.User.NickName, post.PostBody, post.RoomId, Helper.ToMiliseconds (post.CreateDate));
            }

            try {
                room = await _messageHandler.FindAndValidateRoom (postmessage.RoomId);
                post = await _messageHandler.CreateAndValidatePost (postmessage);
                receivers = _messageHandler.FindReceivers (room).ToList ();

                // SUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded += UserAddedEvent;

                await SendMessage (room.IsPublic ? _hubContext.Clients.All : _hubContext.Clients.Users (receivers));

                ///
                /// DANGER ZONE
                ///

                // Simulate slow Db save 
                // TODO : create test 
                 Thread.Sleep (20000);
                await _messageHandler.SavePost (post);
                System.Console.WriteLine ("post saved");

            } catch (Exception ex) {
                return await Error (ex.Message);
            } finally {
                System.Console.WriteLine ("FINALLY!");
                // UNSUBSCRIBE TO EVENT
                _hubLogger._connections.UserAdded -= UserAddedEvent;
            }
            return Ok ();
        }

        [NonAction]
        public async Task<IActionResult> Error (string message) {
            return BadRequest (message);
        }

    }

    public class Postmessage {
        public string Message { get; set; }

        public int RoomId { get; set; }
    }

}