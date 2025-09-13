using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class CreateVoteRequest
    {
        public CreateVoteRequest(Guid userId, Guid? postId, Guid? commentId, VoteType voteType)
        {
            UserId = userId;
            PostId = postId;
            CommentId = commentId;
            VoteType = voteType;
        }

        public Guid UserId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
