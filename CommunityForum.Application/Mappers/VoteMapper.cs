using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class VoteMapper
    {
        public static VoteResponseDTO ToResponse(this Vote vote)
        {
            return new VoteResponseDTO(vote.Id, vote.UserId, vote.PostId, vote.CommentId, vote.VoteType);
        }
    }
}
