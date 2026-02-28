using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public Guid UserId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        [Required]
        [EnumDataType(typeof(VoteType), ErrorMessage = "Such vote does not exist!")]
        public VoteType VoteType { get; set; }
    }
}
