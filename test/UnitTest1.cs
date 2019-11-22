using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using app.Models;
using app.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalRChat.Hubs;
using Xunit;
using Xunit.Abstractions;

namespace test
{

    public class ChatHubTest1
    {
        public ChatHubTest1(ITestOutputHelper output)
        {
            this.output = output;

            var hubInst = new IHubContextMethods();
            this.room = hubInst.GetRoom();
            this.post = hubInst.GetPost();
            this.postmessage = hubInst.GetPostMessage();
            this.receivers = hubInst.GetReceivers();
             this.UserIdentifierString = hubInst.UserIdentifier();

            mockRepo = new Mock<IHubRepository> ();
            mockRepo.Setup (repo => repo.FindAndValidateRoom (1, UserIdentifierString)).ReturnsAsync(room);
            mockRepo.Setup (repo => repo.CreateAndValidatePost (postmessage, UserIdentifierString)).Returns(post);
            mockRepo.Setup (repo => repo.FindReceivers (room)).Returns (receivers);
            mockClients = new Mock<IHubCallerClients>();
            mockCallerContext = new Mock<HubCallerContext>();
            mockClientProxy = new Mock<IClientProxy>();
        }
        private readonly ITestOutputHelper output;
        Mock<IHubRepository> mockRepo;
        Mock<IHubCallerClients> mockClients;

        Mock<HubCallerContext> mockCallerContext;
        Mock<IClientProxy> mockClientProxy;

        public Room room { get; }
        public Post post { get; }
        public Postmessage postmessage { get; }
        public List<string> receivers { get; }

        public string UserIdentifierString { get; }

      

        [Fact]
        public async Task Should_Create_Users_When_Room_Is_NOT_Public()
        {
            // Arrange
            room.IsPublic = false;
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);
            mockCallerContext.Setup(clients => clients.UserIdentifier).Returns(UserIdentifierString);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object,
                Context = mockCallerContext.Object
            };
            await chathub.SendMessage(postmessage);


            // Assert
            // Really calls SendAsync which works as an extension method
            mockClients.Verify(clients => clients.Users(receivers), Times.Once);
        }

        [Fact]
        public async Task Should_Create_All_When_Room_Is_Public()
        {
            // Arrange
            ///
            // NOTICE : room should be public for it to create All Class
            ///
            room.IsPublic = true;

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockCallerContext.Setup(clients => clients.UserIdentifier).Returns(UserIdentifierString);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object,
                Context = mockCallerContext.Object
            };
            await chathub.SendMessage(postmessage);

            // Assert
            mockClients.Verify(clients => clients.All, Times.Once);

        }

        [Fact]
        public async Task Should_Create_Users_When_Room_Is_NOT_Public_And_Never_All()
        {
            // Arrange
            room.IsPublic = false;
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);
            mockCallerContext.Setup(clients => clients.UserIdentifier).Returns(UserIdentifierString);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object,
                 Context = mockCallerContext.Object
            };
            await chathub.SendMessage(postmessage);

            // Assert
            mockClients.Verify(clients => clients.All, Times.Never());
            mockClients.Verify(clients => clients.Users(receivers), Times.Once);
        }

        [Fact]
        public async Task Should_Execute_SendCoreAsync_Method()
        {
            // Arrange
            room.IsPublic = false;
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);
            mockCallerContext.Setup(clients => clients.UserIdentifier).Returns(UserIdentifierString);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object,
                Context = mockCallerContext.Object
            };
            await chathub.SendMessage(postmessage);

            // Assert
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

        [Fact]
        public async Task Should_Execute_SendCoreAsync_Twice_When_OnConnectedAsync_Executed_Before_SendMessage_Returns()
        {
            // This test simulates the wssocket call to to Chathub.SendMessage where user connects before message is saved to database.
            // The OnConnectedAsync method is fired on user connection. Stored delegate is then executed and wsmessage method SendCoreAsync is executed again.   

            // Arrange
            room.IsPublic = true;
            mockClients.Setup(clients => clients.User(UserIdentifierString)).Returns(mockClientProxy.Object);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockRepo.Setup(repo => repo.SavePost(post)).Returns(Task.Factory.StartNew(() => {
                // Simulate slow db call
                 Thread.Sleep(2000);
                 return true;
            }));

            mockCallerContext.Setup(clients => clients.UserIdentifier).Returns(UserIdentifierString);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object,
                Context = mockCallerContext.Object
            };
              Task task1 = Task.Factory.StartNew(() => chathub.SendMessage(postmessage));
              Task task2 = Task.Factory.StartNew(async () => {
                  Thread.Sleep(1000);
                  // Wait, then execute OnConnectedAsync on user added
                  await chathub.OnConnectedAsync();
              });
              Task.WaitAll(task1, task2); 

            // Assert
            // Assert that the function is called twice
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(2));

             mockRepo.Verify(clientProxy => clientProxy.SavePost(post), Times.Exactly(1));

        }

    }
}
