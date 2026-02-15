using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class PostTagMapper
    {
        public static PostTagResponseDTO ToResponse(this PostTag postTag)
        {
            return new PostTagResponseDTO(postTag.PostId, postTag.TagId, postTag.Tag?.Name ?? string.Empty);
        }
    }
}
