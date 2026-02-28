using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        [Required]
        [EnumDataType(typeof(VoteType), ErrorMessage = "Such vote does not exist!")]
        public VoteType VoteType { get; set; }

        [Required]
        public DateTime VotedAt { get; set; }

        [Required]
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
