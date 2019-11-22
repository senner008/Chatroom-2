using System;
using System.Collections.Generic;
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

    }
}
