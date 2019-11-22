using System;
using System.Collections.Generic;
using app.Controllers;
using app.Models;
using app.Repositories;
using Moq;
using SignalRChat.Hubs;

namespace test
{
    public class IHubContextMethods {
        public Room GetRoom () 
        {
            var room = new Room 
            {
                Id = 1,
                Name = "my room",
                IsPublic = false,
                UsersLink = new List<UserRoom> 
                {
                    new UserRoom 
                    {
                        UserId = "user1",
                        RoomId = 1
                    },
                    new UserRoom 
                    {
                        UserId = "user2",
                        RoomId = 1
                    }
                }
            };
            return room;
        }

        public Post GetPost () {
            var user = new ApplicationUser ();
            user.NickName = "MyNickname";
            return new Post {
                Id = 1,
                    PostBody = "lorem",
                    CreateDate = DateTime.Now,
                    RoomId = 1,
                    UserId = "user1",
                    User = user
            };
        }

        public Postmessage GetPostMessage () {
            return new Postmessage { Message = "Hello", RoomId = 1 };
        }

        public string UserIdentifier () {
            return "stringUser1";
        }

        public List<string> GetReceivers () {

            return new List<string> () { "user1", "user2" };
        }

    }


}