using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class UserMapper
    {
        public static UserResponseDTO ToResponse(this User user)
        {
            return new UserResponseDTO(user.Id, user.Username, user.Email, user.Role);
        }
    }
}
