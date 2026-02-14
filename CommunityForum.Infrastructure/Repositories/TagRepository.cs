using CommunityForum.Domain.Entities;
using CommunityForum.Domain.Interfaces;
using CommunityForum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _dbContext;

        public TagRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Tag tag)
        {
            await _dbContext.Tags.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _dbContext.Tags.FindAsync(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Tag>> GetAllAsync()
        {
            return await _dbContext.Tags
                .Include(tag => tag.PostTags)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Tags
                .Include(tag => tag.PostTags)
                .FirstOrDefaultAsync(tag => tag.Id == id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _dbContext.Tags
                .Include(tag => tag.PostTags)
                .FirstOrDefaultAsync(tag => tag.Name.ToLower() == name.ToLower());
        }

        public async Task UpdateAsync(Tag tag)
        {
            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync();
        }
    }
}
