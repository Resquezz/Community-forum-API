using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class PostResponseDTO
    {
        public PostResponseDTO(Guid id, string content, Guid userId, string username, Guid topicId, string topicTitle,
            DateTime createdAt = default, int upvotesCount = 0, int downvotesCount = 0, int score = 0, int commentsCount = 0)
        {
            Id = id;
            Content = content;
            UserId = userId;
            Username = username;
            TopicId = topicId;
            TopicTitle = topicTitle;
            CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt;
            UpvotesCount = upvotesCount;
            DownvotesCount = downvotesCount;
            Score = score;
            CommentsCount = commentsCount;
        }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Content { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        public Guid TopicId { get; set; }

        [Required]
        [StringLength(100)]
        public string TopicTitle { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int UpvotesCount { get; set; }

        [Required]
        public int DownvotesCount { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public int CommentsCount { get; set; }
        public override bool Equals(object? obj)
        {
            if(obj is not PostResponseDTO other)
                return false;
            return Id == other.Id && Content == other.Content && UserId == other.UserId
                && Username == other.Username && TopicId == other.TopicId && TopicTitle == other.TopicTitle
                && CreatedAt == other.CreatedAt && UpvotesCount == other.UpvotesCount
                && DownvotesCount == other.DownvotesCount && Score == other.Score
                && CommentsCount == other.CommentsCount;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Content, UserId, Username, TopicId, TopicTitle,
                CreatedAt, UpvotesCount);
        }
    }
}
