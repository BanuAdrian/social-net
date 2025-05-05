using System.ComponentModel.DataAnnotations.Schema;

namespace social_net.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GroupMembership> GroupMemberships { get; set; } = new();

        [NotMapped]
        public List<User> Members => GroupMemberships.Select(gm => gm.Member).ToList();

        public List<GroupMessage> ReceivedMessages { get; set; } = new();
    }
}
