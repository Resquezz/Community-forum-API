using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface IVoteRepository
    {
        Task AddAsync(Vote vote);
        Task UpdateAsync(Vote vote);
        Task DeleteAsync(Guid id);
        Task<Vote?> GetByIdAsync(Guid id);
    }
}
