
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Identity;

public class Utilities 
{
    public UserManager<ApplicationUser> UserManager { get; }

    public Utilities( UserManager<ApplicationUser> userManager)
    {
        UserManager = userManager;
    }

    public async void InitializeDbForTests(ApplicationDbContext db)
    {
        db.Posts.Add(await GetSeedingMessages());
        db.SaveChanges();
    }

    public void ReinitializeDbForTests(ApplicationDbContext db)
    {
        db.Posts.RemoveRange(db.Posts);
        InitializeDbForTests(db);
    }

    public async Task<Post> GetSeedingMessages()
    {






        // List<Post> Posts = new List<Post>();

        // int rowsToInsert = 10;

        // for (var i = 1; i < rowsToInsert; i++) 
        // {
        //     Posts.Add(new Post 
        //     { 
        //         Id = i, 
        //         UserId = "userId1", 
        //         RoomId = 1, 
        //         PostBody = "bla bla bla...", 
        //         CreateDate = DateTime.Now,
        //         Identifier = Guid.NewGuid()
        //     });
        // }
        // return Posts;


        return new Post 
            { 
                Id = 1,
                UserId = "395c3e26-72ea-4917-bddd-8a553d308e49",  
                RoomId = 1, 
                PostBody = "bla bla bla...", 
                CreateDate = DateTime.Now,
                Identifier = Guid.NewGuid()
            };
    }
}