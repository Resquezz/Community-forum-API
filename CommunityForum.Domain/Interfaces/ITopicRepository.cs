using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface ITopicRepository
    {
        Task AddAsync(Topic topic);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(Guid id);
        Task<Topic?> GetByIdAsync(Guid id);
        Task<ICollection<Topic>> GetAllAsync();
    }
}
