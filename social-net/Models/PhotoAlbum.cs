namespace social_net.Models
{
    public class PhotoAlbum
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime PostedAt { get; set; }

        public User User { get; set; }
        public List<Photo> Photos { get; set; } = new();
    }
}
