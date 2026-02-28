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
        public CreateVoteRequest(Guid? postId, Guid? commentId, VoteType voteType)
        {
            PostId = postId;
            CommentId = commentId;
            VoteType = voteType;
        }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        [Required]
        [EnumDataType(typeof(VoteType), ErrorMessage = "Such vote does not exist!")]
        public VoteType VoteType { get; set; }
    }
}
