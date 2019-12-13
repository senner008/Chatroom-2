## Notes:
- https://stackoverflow.com/questions/55473852/how-to-test-asp-net-core-web-api-with-cookie-authentication-using-postman


## TODO PRIORITY :
- Add properties to IdentityUser - ICollection<UserRoom>, ApplicationUser from IdentityUser - DONE
- User NickName to registration - DONE
- Validate room param against user - DONE
- Seed initial users - DONE
- Deploy to Heroku - DONE
- Setup CI chain, review apps - DONE
- check validationToken on all httppost - DONE
- Use Ef Core BulkInsert - https://www.nuget.org/packages/Z.EntityFramework.Extensions.EFCore/ - DONE
- hide input on roomId null - DONE
- Typescript and webpack - DONE
- Redirect to global not found page - DONE
- notify on connect/disconnect - DONE
- add connection state to State object - DONE
- add reconnect button - DONE
- loading spinners - Partinally DONE
- handle, test and illustrate concurrency issue - DONE
- Client side tests using jest - PARTIALLY DONE
- Dynamic room creation - DONE
- XUnit tests - PARTIALLY DONE - ADD MORE
- Move logic in connection callback to action methods. spilt into logic and render methods - DONE
- handle init in razor. use dynamic room routing - DONE
- send room over ws and push to room list - DONE
- add validation - DONE 
- create unique identifier for postsStorage keys - DONE
- HTMLSanitizer to global filter - DONE
- global exception handling, return a redirect in ws message on db error - DONE
- controller tests, integration tests - DONE
- fix scrolling layout - DONE
- add content security policies - DONE
- add scroll to rooms - DONE
- fix when no room route
- remove logic from listeners
- add random thread access test : https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement
- Dockerize



## TODO SECONDARY : 
- implement "load more messages"
- Show other users connection status
- Edit/Delete previous posts
- Encryption
- Runtime table creation using raw sql
- simulate user interaction using puppeteer
- hide rooms with no access on init rooms get
- fix client side logging

## FEATURES TODO : 
- Giphy insertion
- Swagger
- Fake user tests
- Spinner on user typing
- Schedule db copy to postsarchive table, add multiple indexes to postsarchive table
- make css responsive

## NOTES:
- _context.Database.EnsureCreated(); adds an extrea 200 ms to the db call
- users only allowed to send one message to hub at a time when not in controller

```
{
  "ConnectionStrings": {
    "CodeToShowDb": {{Test}
  },
  "Passwords" : {
    "adminpass" : {{USER SEEDS PASSWORD}}
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
## EF NOTES 1:

```
_context.Posts
    .Where(post => post.RoomId == id)
    .OrderByDescending(post => post.Id)
    .Take(100)
    // .Include(post => post.Room)
    // .ThenInclude(room => room.UsersLink)
    .Select(post => new PostModel { 
        PostBody = post.PostBody, 
        UserName = post.User.NickName, 
        CreateDate = post.CreateDate,
        RoomId = post.RoomId,
        Identifier = post.Identifier.ToString()
    })
    .AsNoTracking()
    .ToListAsync();

 SELECT `t`.`PostBody`, `a`.`NickName` AS `UserName`, `t`.`CreateDate`, `t`.`RoomId`, CONVERT(`t`.`Identifier`, CHAR(36)) AS `Identifier`
      FROM (
          SELECT `p`.`Id`, `p`.`CreateDate`, `p`.`Identifier`, `p`.`Likes`, `p`.`PostBody`, `p`.`RoomId`, `p`.`UpdateDate`, `p`.`UserId`
          FROM `Posts` AS `p`
          WHERE (`p`.`RoomId` = @__id_0) AND @__id_0 IS NOT NULL
          ORDER BY `p`.`Id` DESC
          LIMIT @__p_1
      ) AS `t`
      INNER JOIN `AspNetUsers` AS `a` ON `t`.`UserId` = `a`.`Id`
      ORDER BY `t`.`Id` DESC
```
## EF NOTES 2: (Limit before sorting)
```
_context.Posts
    .Where(post => post.RoomId == id)
    .OrderByDescending(post => post.Id)
    // .Include(post => post.Room)
    // .ThenInclude(room => room.UsersLink)
    .Select(post => new PostModel { 
        PostBody = post.PostBody, 
        UserName = post.User.NickName, 
        CreateDate = post.CreateDate,
        RoomId = post.RoomId,
        Identifier = post.Identifier.ToString()
    })
    .AsNoTracking()
    .Take(100)
    .ToListAsync();

  SELECT `p`.`PostBody`, `a`.`NickName` AS `UserName`, `p`.`CreateDate`, `p`.`RoomId`, CONVERT(`p`.`Identifier`, CHAR(36)) AS `Identifier`
      FROM `Posts` AS `p`
      INNER JOIN `AspNetUsers` AS `a` ON `p`.`UserId` = `a`.`Id`
      WHERE (`p`.`RoomId` = @__id_0) AND @__id_0 IS NOT NULL
      ORDER BY `p`.`Id` DESC
      LIMIT @__p_1
```