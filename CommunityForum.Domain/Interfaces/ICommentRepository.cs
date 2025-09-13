﻿using CommunityForum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Guid id);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<ICollection<Comment>> GetAllAsync();
    }
}
