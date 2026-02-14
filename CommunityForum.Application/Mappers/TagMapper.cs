using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class TagMapper
    {
        public static TagResponseDTO ToResponse(this Tag tag)
        {
            return new TagResponseDTO(tag.Id, tag.Name);
        }
    }
}
