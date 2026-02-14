using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface ITagService
    {
        Task<TagResponseDTO> CreateTagAsync(CreateTagRequest request);
        Task<TagResponseDTO> UpdateTagAsync(UpdateTagRequest request);
        Task DeleteTagAsync(DeleteTagRequest request);
        Task<ICollection<TagResponseDTO>?> GetAllTagsAsync();
        Task<TagResponseDTO> GetTagByIdAsync(Guid id);
    }
}
