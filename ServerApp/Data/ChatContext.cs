using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServerApp.models;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class ChatContext:IdentityDbContext<User,Role,int>
    {
        public ChatContext(DbContextOptions<ChatContext> options):base(options)
        {
            
        }
                public DbSet<Group> Groups{ get; set; }

                public DbSet<Message> Messages{ get; set; }

                public DbSet<GroupUser> GroupUsers{ get; set; }
        
        


          protected override void OnModelCreating(ModelBuilder builder) 
        {
          base.OnModelCreating(builder);
         
                builder.Entity<GroupUser>()
    .HasKey(gu => new { gu.GroupId, gu.UserId });


             builder.Entity<User>()
        .HasMany(u => u.ReceivedMessages)
        .WithOne(m => m.Receiver)
        .HasForeignKey(m => m.ReceiverId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<User>()
        .HasMany(u => u.GroupUsers)
        .WithOne(gu => gu.User)
        .HasForeignKey(gu => gu.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Group>()
        .HasMany(g => g.Messages)
        .WithOne(m => m.Group)
        .HasForeignKey(m => m.GroupId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Group>()
        .HasMany(g => g.GroupUsers)
        .WithOne(gu => gu.Group)
        .HasForeignKey(gu => gu.GroupId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany()
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Message>()
        .HasOne(m => m.Group)
        .WithMany()
        .HasForeignKey(m => m.GroupId)
        .OnDelete(DeleteBehavior.Restrict);


            
        }
 

    }
}