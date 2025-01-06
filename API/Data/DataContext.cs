﻿using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; } = null!;
    public DbSet<UserLike> Likes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
            .HasKey(key => new { key.SourceUserId, key.TargetUserId });

        builder.Entity<UserLike>()
            .HasOne(source => source.SourceUser)
            .WithMany(like => like.LikedUsers)
            .HasForeignKey(fk => fk.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(source => source.TargetUser)
            .WithMany(like => like.LikedByUsers)
            .HasForeignKey(fk => fk.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
