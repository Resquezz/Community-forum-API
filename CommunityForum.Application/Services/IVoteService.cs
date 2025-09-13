using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface IVoteService
    {
        Task<VoteResponseDTO> CreateVoteAsync(CreateVoteRequest request);
        Task<VoteResponseDTO> UpdateVoteAsync(UpdateVoteRequest request);
        Task DeleteVoteAsync(DeleteVoteRequest request);
        Task<VoteResponseDTO> GetVoteByIdAsync(Guid id);
    }
}
