namespace social_net.Models
{
    public class GroupMembership
    {
        public int GroupId { get; set; }
        public string UserId { get; set; }

        public User Member { get; set; }
        public Group Group { get; set; }
    }
}
