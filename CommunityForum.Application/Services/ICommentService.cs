using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface ICommentService
    {
        Task<CommentResponseDTO> CreateCommentAsync(CreateCommentRequest request);
        Task<CommentResponseDTO> UpdateCommentAsync(UpdateCommentRequest request);
        Task DeleteCommentAsync(DeleteCommentRequest request);
        Task<ICollection<CommentResponseDTO>>? GetAllCommentsAsync();
        Task<CommentResponseDTO> GetCommentByIdAsync(Guid id);
    }
}
