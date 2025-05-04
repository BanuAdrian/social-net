using System.ComponentModel.DataAnnotations.Schema;

namespace social_net.Models
{
    public class FriendRequest
    {
        //public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        //public DateTime RequestDate { get; set; }
        
        //[ForeignKey(nameof(SenderId))]
        public User Sender { get; set; }

        //[ForeignKey(nameof(ReceiverId))]
        public User Receiver { get; set; }
    }
}
