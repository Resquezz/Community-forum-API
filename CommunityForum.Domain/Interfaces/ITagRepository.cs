using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface ITagRepository
    {
        Task AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag?> GetByNameAsync(string name);
        Task<ICollection<Tag>> GetAllAsync();
    }
}
