using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponseDTO> UpdateCategoryAsync(UpdateCategoryRequest request);
        Task DeleteCategoryAsync(DeleteCategoryRequest request);
        Task<ICollection<CategoryResponseDTO>?> GetAllCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(Guid id);
    }
}
