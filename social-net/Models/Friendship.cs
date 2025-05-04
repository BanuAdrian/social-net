namespace social_net.Models
{
    public class Friendship
    {
        public string InitiatorUserId { get; set; }
        public string RecipientUserId { get; set; }

        public User InitiatorUser { get; set; }
        public User RecipientUser { get; set; }
    }
}
