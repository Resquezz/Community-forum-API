using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.DTOs.RequestDTOs
{
    public class UpdateVoteRequest
    {
        public UpdateVoteRequest(Guid id, VoteType voteType)
        {
            Id = id;
            VoteType = voteType;
        }

        [Required]
        public Guid Id { get; set; }

        [EnumDataType(typeof(VoteType), ErrorMessage = "Such vote does not exist!")]
        public VoteType VoteType { get; set; }
    }
}
