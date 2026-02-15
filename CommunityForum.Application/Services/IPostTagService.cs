using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface IPostTagService
    {
        Task<PostTagResponseDTO> AddPostTagAsync(CreatePostTagRequest request);
        Task DeletePostTagAsync(DeletePostTagRequest request);
        Task<ICollection<TagResponseDTO>> GetTagsByPostIdAsync(Guid postId);
    }
}
