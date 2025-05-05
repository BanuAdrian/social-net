namespace social_net.Models
{
    public class PhotoComment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PhotoId { get; set; }
        public string Text { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime AddedAt { get; set; }
        public Photo Photo { get; set; }
        public User User { get; set; }
    }
}
