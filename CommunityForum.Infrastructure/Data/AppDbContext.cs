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
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasIndex(category => category.Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .Property(category => category.Name)
                .HasMaxLength(80)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(category => category.Description)
                .HasMaxLength(250)
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .HasIndex(tag => tag.Name)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .Property(tag => tag.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Topic>()
                .Property(topic => topic.Title)
                .HasMaxLength(120)
                .IsRequired();

            modelBuilder.Entity<Topic>()
                .Property(topic => topic.Description)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<PostTag>()
                .HasKey(postTag => new { postTag.PostId, postTag.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(postTag => postTag.Post)
                .WithMany(post => post.PostTags)
                .HasForeignKey(postTag => postTag.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostTag>()
                .HasOne(postTag => postTag.Tag)
                .WithMany(tag => tag.PostTags)
                .HasForeignKey(postTag => postTag.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Topics)
                .WithOne(topic => topic.User)
                .HasForeignKey(topic => topic.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(category => category.Topics)
                .WithOne(topic => topic.Category)
                .HasForeignKey(topic => topic.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

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
