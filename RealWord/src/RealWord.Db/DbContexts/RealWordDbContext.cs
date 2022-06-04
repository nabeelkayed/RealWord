using Microsoft.EntityFrameworkCore;
using RealWord.Db.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealWord.Db
{
    public class RealWordDbContext : DbContext
    {
        public RealWordDbContext(DbContextOptions<RealWordDbContext> options)
          : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleFavorites> ArticleFavorites { get; set; }
        public DbSet<ArticleTags> ArticleTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFollowers> UserFollowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(a =>
            {
                a.HasKey(a => a.ArticleId);

                //a.Property<DateTime>("CreatedAt");
                //a.Property<DateTime>("UpdatedAt");

                a.Property(a => a.Slug).IsRequired();
                a.Property(a => a.Title).IsRequired();

                a.Property(a => a.Body).IsRequired();
                a.Property(a => a.Description).IsRequired();

                a.Property(a => a.CreatedAt).IsRequired();
                a.Property(a => a.UpdatedAt).IsRequired();

                a.HasMany<Comment>(a => a.Comments)
               .WithOne(c => c.Article)
               .HasForeignKey(c => c.ArticleId)
               .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ArticleFavorites>(a =>
            {
                a.HasKey(af => new { af.ArticleId, af.UserId });

                a.HasOne(ua => ua.Article)
                .WithMany(u => u.Favorites)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

                a.HasOne(u => u.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ArticleTags>(a =>
            {
                a.HasKey(at => new { at.ArticleId, at.TagId });

                a.HasOne(ua => ua.Article)
                .WithMany(u => u.Tags)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

                a.HasOne(u => u.Tag)
                .WithMany(u => u.Articles)
                .HasForeignKey(u => u.TagId)
                .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<Comment>(c =>
            {
                c.HasKey(c => c.CommentId);

                //c.Property<DateTime>("CreatedAt");
                //c.Property<DateTime>("UpdatedAt");

                c.Property(c => c.Body).IsRequired();
                c.Property(c => c.CreatedAt).IsRequired();
                c.Property(c => c.UpdatedAt).IsRequired();

            });


            modelBuilder.Entity<Tag>(t =>
            {
                t.HasKey(t => t.TagId);
            });

            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(u => u.UserId);

                u.HasIndex(u => u.Email).IsUnique();
                u.HasIndex(u => u.Username).IsUnique();

                u.Property(a => a.Email).IsRequired();
                u.Property(a => a.Password).IsRequired();
                u.Property(a => a.Username).IsRequired();

                u.HasMany<Article>(a => a.Articles)
                .WithOne(u => u.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

                u.HasMany<Comment>(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserFollowers>(u =>
            {
                u.HasKey(u => new { u.FollowerId, u.FolloweingId });

                u.HasOne(ua => ua.Follower)
                .WithMany(u => u.Followerings)
                .HasForeignKey(ua => ua.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

                u.HasOne(u => u.Followeing)
                .WithMany(u => u.Followers)
                .HasForeignKey(u => u.FolloweingId)
                .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
