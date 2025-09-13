using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Guid id);
        Task<Post?> GetByIdAsync(Guid id);
        Task<ICollection<Post>> GetAllAsync();
    }
}
