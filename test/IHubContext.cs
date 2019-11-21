using System;
using System.Collections.Generic;
using app.Controllers;
using app.Models;
using app.Repositories;
using Moq;
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

        public List<string> GetReceivers () {

            return new List<string> () { "user1", "user2" };
        }

    }
    public class ChatHubMethodObjects {
        public ChatHubMethodObjects()
        {
            var hubInst = new IHubContextMethods();
            this.room = hubInst.GetRoom();
            this.post = hubInst.GetPost();
            this.postmessage = hubInst.GetPostMessage();
            this.receivers = hubInst.GetReceivers();
        }

        public Room room { get; } 
        public Post post { get; } 
        public Postmessage postmessage { get; } 
        public List<string> receivers { get; }

    }
    public class GetMock {
        public static Mock<IHubRepository> SetupIHubRespository (Room room, Post post, Postmessage postmessage, List<string> receivers) {
            // create suitable note / subversion objects 
            // either by passing them in or new-ing them up directly with default values. 
            var Repo = new Mock<IHubRepository> ();
            Repo.Setup (repo => repo.FindAndValidateRoom (1)).ReturnsAsync (room);
            Repo.Setup (repo => repo.CreateAndValidatePost (postmessage)).ReturnsAsync (post);
            Repo.Setup (repo => repo.FindReceivers (room)).Returns (receivers);

            return Repo;
        }
    }

}