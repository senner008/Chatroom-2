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
- handle, test and illustrate concurrency issue - PARTIALLY DONE
- Client side tests using jest - PARTIALLY DONE
- Dynamic room creation - DONE
- handle init in razor. use dynamic room routing
- allow user readonly roles for testing. create automated bot testing
- add validation
- Move logic in connection callback to action methods. spilt into logic and render methods
- remove logic from listeners
- send room over ws and push to room list
- XUnit tests
- Show other users connection status
- Edit/Delete previous posts
- HTMLSanitizer to global filter
- Encryption
- log everything client and server side
- add content security policies
- implement "load more messages"
- create unique identifier for postsStorage keys - Guid
- fix client side logging
- clean up css
- split render methods

## TODO SECONDARY : 
- Runtime table creation using raw sql
- simulate user interaction using puppeteer
- hide rooms with no access on init rooms get 

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
- 

![Db issue unsolved](https://github.com/senner008/Chatroom-2/blob/master/dbissue.png)
![Db issue unsolved](https://github.com/senner008/Chatroom-2/blob/master/dbissue_solved.png)


```
{
  "ConnectionStrings": {
    "CodeToShowDb": {{CNSTRING}
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
