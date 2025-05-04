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
    }
}
