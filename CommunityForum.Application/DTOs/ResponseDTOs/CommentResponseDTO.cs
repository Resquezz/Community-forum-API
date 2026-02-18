using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class CommentResponseDTO
    {
        public CommentResponseDTO(Guid id, string content, Guid userId, Guid postId, Guid? parentCommentId,
            DateTime createdAt = default, int upvotesCount = 0, int downvotesCount = 0, int score = 0, int repliesCount = 0)
        {
            Id = id;
            Content = content;
            UserId = userId;
            PostId = postId;
            ParentCommentId = parentCommentId;
            CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt;
            UpvotesCount = upvotesCount;
            DownvotesCount = downvotesCount;
            Score = score;
            RepliesCount = repliesCount;
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpvotesCount { get; set; }
        public int DownvotesCount { get; set; }
        public int Score { get; set; }
        public int RepliesCount { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not CommentResponseDTO other)
                return false;
            return Id == other.Id && Content == other.Content && UserId == other.UserId && PostId == other.PostId
                && ParentCommentId == other.ParentCommentId && CreatedAt == other.CreatedAt
                && UpvotesCount == other.UpvotesCount && DownvotesCount == other.DownvotesCount
                && Score == other.Score && RepliesCount == other.RepliesCount;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Content, UserId, PostId, ParentCommentId,
                CreatedAt, UpvotesCount, DownvotesCount);
        }
    }
}
