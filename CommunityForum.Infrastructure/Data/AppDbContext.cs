using CommunityForum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Vote> Votes => Set<Vote>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Topic>()
                .HasMany(topic => topic.Posts)
                .WithOne(post => post.Topic)
                .HasForeignKey(post => post.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasMany(post => post.Votes)
                .WithOne(vote => vote.Post)
                .HasForeignKey(vote => vote.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.ParentComment)
                .WithMany(pComment => pComment.Replies)
                .HasForeignKey(pComment => pComment.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasMany(comment => comment.Votes)
                .WithOne(vote => vote.Comment)
                .HasForeignKey(vote => vote.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasOne(vote => vote.User)
                .WithMany()
                .HasForeignKey(vote => vote.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .Property(vote => vote.VoteType)
                .IsRequired();
        }
    }
}
