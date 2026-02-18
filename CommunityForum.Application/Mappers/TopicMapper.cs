using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class TopicMapper
    {
        public static TopicResponseDTO ToResponse(this Topic topic)
        {
            return new TopicResponseDTO(topic.Id, topic.Title, topic.Description,
                topic.CategoryId, topic.Category?.Name ?? string.Empty, topic.CreatedAt);
        }
    }
}
