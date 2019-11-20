using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using app.Controllers;
using app.Models;
using app.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalRChat.Hubs;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        private Room GetRoom()
        {
            var room = new Room
            {
                Id = 1,
                Name = "my room",
                IsPublic = false,
                UsersLink = new List<UserRoom>
                {
                    new UserRoom { 
                        UserId = "user1", 
                        RoomId = 1
                    },
                    new UserRoom { 
                        UserId = "user2", 
                        RoomId = 1
                    }
                }
            };
           return room;
        }

        private Post GetPost()
        {
            var user = new ApplicationUser();
            user.NickName = "Buller";
            return new Post { Id = 1, PostBody = "bdsfds" , CreateDate = DateTime.Now, RoomId = 1, UserId = "user1", User = user};
        }

        private Postmessage GetPostMessage()
        {
            return new Postmessage { Message = "Buller", RoomId = 1};
        }

        private List<string> GetReceivers()
        {

            return new List<string> () { "dsads", "dsadsa" };
        }

        [Fact]
        public async Task Should_Execute_SignalR_Clients_Users_Method()
        {
            // arrange
            var mockRepo = new Mock<IHubRepository>();
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();

            var postmessage = GetPostMessage();
            var receivers = GetReceivers();
            var room = GetRoom(); 
            ///
            // NOTICE : room should NOT be public for it to execute Users method
            ///
            var post = GetPost();
            mockRepo.Setup(repo => repo.FindAndValidateRoom(1)).ReturnsAsync(room);
            mockRepo.Setup(repo => repo.CreateAndValidatePost(postmessage)).ReturnsAsync(post);
            mockRepo.Setup(repo => repo.FindReceivers(room)).Returns(receivers);
 
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);

 
            ChatHub simpleHub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object
            };
 
            // act
            await simpleHub.SendMessage(postmessage);

             mockClients.Verify(clients => clients.Users(receivers), Times.Once);
 
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

         [Fact]
        public async Task Should_Execute_SignalR_Clients_All_Method()
        {
            // arrange
            var mockRepo = new Mock<IHubRepository>();
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();

            var postmessage = GetPostMessage();
            var receivers = GetReceivers();
            var room = GetRoom(); 
            ///
            // NOTICE : room should be public for it to execute All method
            ///
            room.IsPublic = true;
            var post = GetPost();
            mockRepo.Setup(repo => repo.FindAndValidateRoom(1)).ReturnsAsync(room);
            mockRepo.Setup(repo => repo.CreateAndValidatePost(postmessage)).ReturnsAsync(post);
            mockRepo.Setup(repo => repo.FindReceivers(room)).Returns(receivers);
 
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            ChatHub simpleHub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object
            };
 
            // act
            await simpleHub.SendMessage(postmessage);
 
            // assert
            mockClients.Verify(clients => clients.All, Times.Once);

            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

    }
}
