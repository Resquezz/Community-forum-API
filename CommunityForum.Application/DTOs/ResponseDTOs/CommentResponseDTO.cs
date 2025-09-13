using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class CommentResponseDTO
    {
        public CommentResponseDTO(Guid id, string content, Guid userId, Guid postId, Guid? parentCommentId)
        {
            Id = id;
            Content = content;
            UserId = userId;
            PostId = postId;
            ParentCommentId = parentCommentId;
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not CommentResponseDTO other)
                return false;
            return Id == other.Id && Content == other.Content && UserId == other.UserId && PostId == other.PostId
                && ParentCommentId == other.ParentCommentId;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Content, UserId, PostId, ParentCommentId);
        }
    }
}
