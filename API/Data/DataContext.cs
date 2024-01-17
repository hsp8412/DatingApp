using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }

    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserLike>()
            .HasKey(ul => new { ul.SourceUserId, ul.TargetUserId });

        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.SourceUser)
            .WithMany(su => su.LikedUsers)
            .HasForeignKey(ul => ul.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
            .HasOne(ul => ul.TargetUser)
            .WithMany(su => su.LikedByUsers)
            .HasForeignKey(ul => ul.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
