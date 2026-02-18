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
        public VoteResponseDTO(Guid id, Guid userId, Guid? postId, Guid? commentId, VoteType voteType,
            DateTime votedAt = default, int numericValue = 0)
        {
            Id = id;
            UserId = userId;
            PostId = postId;
            CommentId = commentId;
            VoteType = voteType;
            VotedAt = votedAt == default ? DateTime.UtcNow : votedAt;
            NumericValue = numericValue == 0
                ? (voteType == VoteType.UpVote ? 1 : -1)
                : numericValue;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime VotedAt { get; set; }
        public int NumericValue { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not VoteResponseDTO other)
                return false;
            return Id == other.Id && UserId == other.UserId && PostId == other.PostId && CommentId == other.CommentId
                && VoteType == other.VoteType && VotedAt == other.VotedAt && NumericValue == other.NumericValue;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, UserId, PostId, CommentId, VoteType, VotedAt, NumericValue);
        }
    }
}
