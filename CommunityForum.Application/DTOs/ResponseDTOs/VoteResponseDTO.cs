using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.ResponseDTOs
{
    public class VoteResponseDTO
    {
        public VoteResponseDTO(Guid id, Guid userId, Guid? postId, Guid? commentId, VoteType voteType)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
            CommentId = commentId;
            VoteType = voteType;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public VoteType VoteType { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not VoteResponseDTO other)
                return false;
            return Id == other.Id && UserId == other.UserId && PostId == other.PostId && CommentId == other.CommentId
                && VoteType == other.VoteType;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserId, PostId, CommentId, VoteType);
        }
    }
}
