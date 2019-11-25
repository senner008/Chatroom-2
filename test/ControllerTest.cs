using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using app;
using app.Controllers;
using app.Models;
using app.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace test 
{
  

    public class UnitTest_Controller_Test 
    {

        private List<PostModel> GetTestSessions()
        {
           List<PostModel> Posts = new List<PostModel>();

            int rowsToInsert = 10;

            for (var i = 1; i < rowsToInsert; i++) 
            {
                Posts.Add(new PostModel
                { 
                    RoomId = 1, 
                    PostBody = "bla bla bla...", 
                    CreateDate = DateTime.Now,
                });
            }
            return Posts;
        }
        [Fact]
        public async Task Home_Controller_Index_Should_Return_ViewResult () 
        {
             // Arrange
            var mockRepo = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockRepo.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Posts_Controller_getPostsByRoomId_Should_Return_List_Of_Posts () 
        {
            // Arrange
            var mockhubRepo = new Mock<IHubRepository>();
            var mockPostsRepo = new Mock<IPostsRepository>();
            mockPostsRepo.Setup(repo => repo.getPostsByRoomId(1)).ReturnsAsync(GetTestSessions());

            var controller = new PostsController(mockhubRepo.Object, mockPostsRepo.Object);

            // Act
            var result = await controller.getPostsByRoomId(1);
            
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

    }
}