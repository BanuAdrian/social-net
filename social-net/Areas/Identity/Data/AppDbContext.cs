using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using social_net.Models;

namespace social_net.Data;

public class AppDbContext : IdentityDbContext<User>
{
    //public DbSet<User> User { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<FriendsMessage> FriendsMessages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMembership> GroupMemberships { get; set; }
    public DbSet<GroupMessage> GroupMessages { get; set; }
    public DbSet<TextPost> TextPosts { get; set; }
    public DbSet<PhotoAlbum> PhotoAlbums { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<PhotoComment> PhotoComments { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<User>().Property(u => u.FirstName).HasMaxLength(50);
        builder.Entity<User>().Property(u => u.LastName).HasMaxLength(50);

        builder.Entity<FriendRequest>().ToTable("FriendRequests");

        builder.Entity<FriendRequest>()
            .HasKey(fr => new { fr.SenderId, fr.ReceiverId });

        builder.Entity<FriendRequest>()
            .HasOne(fr => fr.Sender)
            .WithMany(u => u.SentFriendRequests)
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendRequest>()
            .HasOne(fr => fr.Receiver)
            .WithMany(u => u.ReceivedFriendRequests)
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Friendship>().ToTable("Friendships");
        builder.Entity<Friendship>()
            .HasKey(fr => new { fr.InitiatorUserId, fr.RecipientUserId });

        //builder.Entity<Friendship>()
        //    .ToTable(t => t.HasCheckConstraint("CK_Friend_UserOneId_LessThan_UserTwoId", "[UserOneId] < [UserTwoId]"));

        builder.Entity<Friendship>()
            .HasOne(fr => fr.InitiatorUser)
            .WithMany(u => u.InitiatedFriendships)
            .HasForeignKey(fr => fr.InitiatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Friendship>()
            .HasOne(fr => fr.RecipientUser)
            .WithMany(u => u.ReceivedFriendships)
            .HasForeignKey(fr => fr.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<FriendsMessage>().ToTable("FriendsMessages");

        builder.Entity<FriendsMessage>()
            .HasOne(msg => msg.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(msg => msg.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FriendsMessage>()
            .HasOne(msg => msg.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(msg => msg.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Group>().ToTable("Groups");
        builder.Entity<GroupMembership>().ToTable("GroupMemberships");
        builder.Entity<GroupMembership>()
            .HasKey(gm => new { gm.GroupId, gm.UserId });

        builder.Entity<GroupMembership>()
            .HasOne(gm => gm.Member)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GroupMembership>()
            .HasOne(gm => gm.Group)
            .WithMany(gr => gr.GroupMemberships)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Restrict);



        builder.Entity<GroupMessage>().ToTable("GroupMessages");

        builder.Entity<GroupMessage>()
            .HasOne(gmsg => gmsg.Sender)
            .WithMany(u => u.GroupsSentMessages)
            .HasForeignKey(gmsg => gmsg.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GroupMessage>()
            .HasOne(gmsg => gmsg.Group)
            .WithMany(g => g.ReceivedMessages)
            .HasForeignKey(gmsg => gmsg.GroupId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<TextPost>().ToTable("TextPosts");

        builder.Entity<TextPost>()
            .HasOne(t => t.User)
            .WithMany(u => u.TextPosts)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PhotoAlbum>().ToTable("PhotoAlbums");
        builder.Entity<Photo>().ToTable("Photos");
        builder.Entity<PhotoComment>().ToTable("PhotoComments");

        builder.Entity<PhotoComment>()
            .HasOne(pc => pc.User)
            .WithMany(u => u.PhotoComments)
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PhotoComment>()
            .HasOne(pc => pc.Photo)
            .WithMany(p => p.Comments)
            .HasForeignKey(pc => pc.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
