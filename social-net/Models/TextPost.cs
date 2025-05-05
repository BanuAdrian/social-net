namespace social_net.Models
{
    public class TextPost
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime PostedAt { get; set; }

        public User User { get; set; }
    }
}
