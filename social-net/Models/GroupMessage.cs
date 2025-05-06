namespace social_net.Models
{
    public class GroupMessage : IMessage
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public User Sender { get; set; }
        public Group Group { get; set; }
    }
}
