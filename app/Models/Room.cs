using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsPublic { get; set; }
        public ICollection<UserRoom> UsersLink { get; set; }
        public ICollection<Post> Posts { get; set; }
    }

    public class RoomCreateModel
    {
        [Required (ErrorMessage ="Please enter a valid room name")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$", ErrorMessage = "Characters are not allowed or room name length exceeded")]
        [HTMLSanitizerError(ErrorMessage = "Invalid input")]
        public string RoomName { get; set; }

        [Required]
        [EnsureListMinimumOne(ErrorMessage = "Please select at least one user for the room")]
        [EnsureMaxTenUsers(ErrorMessage = "A maximum of 10 users are allowed in private rooms")]
        [EnsureUserNamesAreStringsAndMaxLength30(ErrorMessage = "User nicknames length exceeded")]
        public List<string> UserList {get; set;}
    }
}