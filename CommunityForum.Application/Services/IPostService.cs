using CommunityForum.Application.DTOs;
using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface IPostService
    {
        Task<PostResponseDTO> CreatePostAsync(CreatePostRequest request);
        Task<PostResponseDTO> UpdatePostAsync(UpdatePostRequest request);
        Task DeletePostAsync(DeletePostRequest request);
        Task<ICollection<PostResponseDTO>?> GetAllPostsAsync();
        Task<PostResponseDTO> GetPostByIdAsync(Guid id);
    }
}
