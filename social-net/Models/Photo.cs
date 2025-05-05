namespace social_net.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        
        public string ImagePath { get; set; }

        public PhotoAlbum Album { get; set; }
        public List<PhotoComment> Comments { get; set; } = new();
    }
}
