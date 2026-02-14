using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface IPostTagRepository
    {
        Task AddAsync(PostTag postTag);
        Task DeleteAsync(Guid postId, Guid tagId);
        Task<PostTag?> GetByIdsAsync(Guid postId, Guid tagId);
        Task<ICollection<PostTag>> GetByPostIdAsync(Guid postId);
        Task<ICollection<PostTag>> GetByTagIdAsync(Guid tagId);
    }
}
