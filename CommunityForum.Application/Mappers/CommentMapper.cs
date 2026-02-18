using CommunityForum.Application.DTOs.ResponseDTOs;
using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponseDTO ToResponse(this Comment comment)
        {
            return new CommentResponseDTO(comment.Id, comment.Content, comment.UserId, comment.PostId,
                comment.ParentCommentId, comment.CreatedAt, comment.UpvotesCount, comment.DownvotesCount,
                comment.Score, comment.RepliesCount);
        }
    }
}
