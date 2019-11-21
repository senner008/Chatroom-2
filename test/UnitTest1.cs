using System.Threading;
using System.Threading.Tasks;
using app.Repositories;
using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalRChat.Hubs;
using Xunit;

namespace test
{
    public class UnitTest1 : ChatHubMethodObjects
    {

        [Fact]
        public async Task Should_Execute_SignalR_Clients_Users_Method_When_Room_Is_NOT_Public() 
        {
            // Arrange
            Mock<IHubRepository> mockRepo = GetMock.SetupIHubRespository(room, post, postmessage, receivers);
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object
            };
 
            await chathub.SendMessage(postmessage);

            // Assert
            mockClients.Verify(clients => clients.Users(receivers), Times.Once);
 
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

          [Fact]
        public async Task Should_Execute_SignalR_Clients_Users_Method_When_Room_Is_NOT_Public_And_Never_All()
        {

            Mock<IHubRepository> mockRepo = GetMock.SetupIHubRespository(room, post, postmessage, receivers);
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            
            mockClients.Setup(clients => clients.Users(receivers)).Returns(mockClientProxy.Object);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object
            };
            await chathub.SendMessage(postmessage);

            // Assert
            mockClients.Verify(clients => clients.All, Times.Never());
            mockClients.Verify(clients => clients.Users(receivers), Times.Once);
 
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

        [Fact]
        public async Task Should_Execute_SignalR_Clients_All_Method_When_Room_Is_Public()
        {

            Mock<IHubRepository> mockRepo = GetMock.SetupIHubRespository(room, post, postmessage, receivers);
            Mock<IHubCallerClients> mockClients = new Mock<IHubCallerClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();

            ///
            // NOTICE : room should be public for it to execute All method
            ///
            room.IsPublic = true;
 
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            // Act
            ChatHub chathub = new ChatHub(mockRepo.Object, new HubLogger())
            {
                Clients = mockClients.Object
            };
            await chathub.SendMessage(postmessage);
 
            // Assert
            mockClients.Verify(clients => clients.All, Times.Once);

            // really calls SendAsync which works as an extension method
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage",
                   It.Is<object[]>(o => o.Length == 4),
                    default(CancellationToken)),
                Times.Exactly(1));
        }

    }
}
