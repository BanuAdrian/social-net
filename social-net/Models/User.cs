using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using social_net.Migrations;

namespace social_net.Models
{
    
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
        
        public bool HasPublicProfile { get; set; }

        public List<FriendRequest> SentFriendRequests { get; set; } = new();
        public List<FriendRequest> ReceivedFriendRequests { get; set; } = new();

        public List<Friendship> InitiatedFriendships { get; set; } = new(); 
        public List<Friendship> ReceivedFriendships { get; set; } = new();

        [NotMapped]
        public List<Friendship> AllFriendships => InitiatedFriendships.Concat(ReceivedFriendships).ToList();
        //Model.AllFriendships.Any(fr => fr.InitiatorUserId == currentUser.Id || fr.RecipientUserId == currentUser.Id

        [NotMapped]
        public List<User> Friends => InitiatedFriendships.Select(fr => fr.RecipientUser).Concat(ReceivedFriendships.Select(fr => fr.InitiatorUser)).ToList();

        public List<FriendsMessage> SentMessages { get; set; } = new();
        public List<FriendsMessage> ReceivedMessages { get; set; } = new();

        public List<GroupMembership> GroupMemberships { get; set; } = new();

        [NotMapped]
        public List<Group> Groups => GroupMemberships.Select(gm => gm.Group).ToList();

        public List<GroupMessage> GroupsSentMessages { get; set; } = new();

        //public List<GroupMessage> GroupsReceivedMessages { get; set; } = new();

        public List<TextPost> TextPosts { get; set; } = new();
    }
}
