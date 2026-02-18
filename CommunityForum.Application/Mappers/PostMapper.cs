using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class PostMapper
    {
        public static PostResponseDTO ToResponse(this Post post, User user, Topic topic)
        {
            return new PostResponseDTO(post.Id, post.Content, user.Id, user.Username, topic.Id, topic.Title,
                post.CreatedAt, post.UpvotesCount, post.DownvotesCount, post.Score, post.CommentsCount);
        }
    }
}
