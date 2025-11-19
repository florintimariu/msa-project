using Microsoft.EntityFrameworkCore;
using TodoSocialApp.Models;

namespace TodoSocialApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Friend> Friends { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Friend entity composite primary key
        modelBuilder.Entity<Friend>()
            .HasKey(f => new { f.UserId, f.FollowedUserId });

        // Configure relationships for Friend entity
        modelBuilder.Entity<Friend>()
            .HasOne(f => f.User)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.FollowedUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Configure Todo entity
        modelBuilder.Entity<Todo>()
            .HasOne(t => t.User)
            .WithMany(u => u.Todos)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Message entity
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
