using CommunityForum.Application.DTOs.RequestDTOs;
using CommunityForum.Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Application.Services
{
    public interface IUserService
    {
        //Task<UserResponseDTO> CreateUserAsync(CreateUserRequest request);
        Task<UserResponseDTO> RegisterUserAsync(RegisterUserRequest request);
        Task<string?> LoginUserAsync(LoginUserRequest request);
        Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequest request);
        Task DeleteUserAsync(DeleteUserRequest request);
        Task<ICollection<UserResponseDTO>?> GetAllUsersAsync();
        Task<UserResponseDTO> GetUserById(Guid id);
    }
}
