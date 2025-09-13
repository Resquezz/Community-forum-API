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
    public class TopicRepository : ITopicRepository
    {
        private readonly AppDbContext _dbContext;

        public TopicRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Topic topic)
        {
            await _dbContext.Topics.AddAsync(topic);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var topic = await _dbContext.Topics.FindAsync(id);
            if (topic != null)
            {
                _dbContext.Topics.Remove(topic);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Topic>> GetAllAsync()
        {
            return await _dbContext.Topics
                .Include(topic => topic.Posts)
                .ToListAsync();
        }

        public async Task<Topic?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Topics
                .Include(topic => topic.Posts)
                .FirstOrDefaultAsync(topic => topic.Id == id);
        }

        public async Task UpdateAsync(Topic topic)
        {
            _dbContext.Topics.Update(topic);
            await _dbContext.SaveChangesAsync();
        }
    }
}
