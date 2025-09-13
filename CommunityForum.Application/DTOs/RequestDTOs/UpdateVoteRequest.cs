using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
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

        public Guid Id { get; set; }
        public VoteType VoteType { get; set; }
    }
}
