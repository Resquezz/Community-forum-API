using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface ITopicService
    {
        Task<TopicResponseDTO> CreateTopicAsync(CreateTopicRequest request);
        Task<TopicResponseDTO> UpdateTopicAsync(UpdateTopicRequest request);
        Task DeleteTopicAsync(DeleteTopicRequest request);
        Task<ICollection<TopicResponseDTO>?> GetAllTopicsAsync();
        Task<TopicResponseDTO> GetTopicByIdAsync(Guid id);
    }
}
