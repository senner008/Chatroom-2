using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Extensions;

public static class SeedData
{
    public static async void SeedApplicationUsers(UserManager<ApplicationUser> userManager, string userName, string nickName, string password)
    {

    
        if (await userManager.FindByEmailAsync(userName) == null)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                NickName = nickName,
                EmailConfirmed = true

            };
            IdentityResult result = userManager.CreateAsync(user, password).Result;

            if (!result.Succeeded) throw new Exception("User could not be created!");

            userManager.AddToRoleAsync(user, "Admin").Wait();
        }


    }
    public async static void SeedApplicationRooms(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.Rooms.Any()) return;
        var alpha = await userManager.FindByEmailAsync("alpha@mail.com");
        var beta = await userManager.FindByEmailAsync("beta@mail.com");
        var gamma = await userManager.FindByEmailAsync("gamma@mail.com");

        List<Room> Rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                Name = "Public",
                IsPublic = true
            },
            new Room
            {
                Id = 2,
                Name = "Public 2",
                IsPublic = true
            },
            new Room
            {
                Id = 3,
                Name = "Public 3",
                IsPublic = true
            },
            new Room
            {
                Id = 4,
                Name = "alphaBeta",
                IsPublic = false,
                UsersLink = new List<UserRoom>
                {
                    new UserRoom { 
                        UserId = alpha.Id, 
                        RoomId = 4
                    },
                    new UserRoom { 
                        UserId = beta.Id, 
                        RoomId = 4
                    }
                }
            },
            new Room
            {
                Id = 5,
                Name = "alphaGamma",
                IsPublic = false,
                UsersLink = new List<UserRoom>
                {
                    new UserRoom { 
                        UserId = alpha.Id, 
                        RoomId = 5
                    },
                    new UserRoom { 
                        UserId = gamma.Id, 
                        RoomId = 5
                    }
                }
            },
             new Room
            {
                Id = 6,
                Name = "betaGamma",
                IsPublic = false,
                UsersLink = new List<UserRoom>
                {
                    new UserRoom { 
                        UserId = beta.Id, 
                        RoomId = 6
                    },
                    new UserRoom { 
                        UserId = gamma.Id, 
                        RoomId = 6
                    }
                }
            }
        };
        
        context.Rooms.AddRange(Rooms);
        context.SaveChanges();

    }

     public async static void SeedApplicationPosts(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string cnString)
    {
        if (context.Posts.Any()) return;
        var user = await userManager.FindByEmailAsync("alpha@mail.com");

        Random random = new Random();
        int randomNumber = random.Next(1, 4);


        List<Post> Posts = new List<Post>();

        int rowsToInsert = 100;

        for (var i = 1; i < rowsToInsert; i++) 
        {
            Posts.Add(new Post 
            { 
                Id = i, 
                UserId = user.Id, 
                RoomId = random.Next(1, 4), 
                PostBody = "bla bla bla...", 
                CreateDate = DateTime.Now,
                Identifier = Guid.NewGuid()
            });
        }
        System.Console.WriteLine("------------------");
        System.Console.WriteLine($"Inserting {rowsToInsert} rows into Posts table...");
        context.Posts.AddRange(Posts);
        context.SaveChanges();
        System.Console.WriteLine($"{rowsToInsert} rows inserted");
        System.Console.WriteLine("------------------");

    }

}