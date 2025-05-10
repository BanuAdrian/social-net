namespace social_net.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsRead { get; set; }

        public User User { get; set; }
    }
}
