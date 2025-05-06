namespace social_net.Models
{
    public interface IMessage
    {
        int Id { get; set; }
        string Content { get; set; }
        DateTime SentAt { get; set; }
        public User Sender { get; set; }
    }
}
