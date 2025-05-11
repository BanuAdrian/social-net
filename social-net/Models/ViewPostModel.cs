using System.ComponentModel.DataAnnotations.Schema;

namespace social_net.Models
{
    [NotMapped]
    public class ViewPostModel
    {
        public DateTime PostedAt { get; set; }
        public string Type { get; set; }    
        public TextPost TextPost {get; set;}
        public PhotoAlbum PhotoAlbum { get; set; }
        public User User { get; set; }

        //public int PostNumber { get; set; }

    }
}
