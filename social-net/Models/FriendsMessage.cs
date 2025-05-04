namespace social_net.Models
{
    public class FriendsMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
