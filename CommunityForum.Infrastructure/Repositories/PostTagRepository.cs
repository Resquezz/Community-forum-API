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
    public class PostTagRepository : IPostTagRepository
    {
        private readonly AppDbContext _dbContext;

        public PostTagRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PostTag postTag)
        {
            await _dbContext.PostTags.AddAsync(postTag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId, Guid tagId)
        {
            var postTag = await _dbContext.PostTags.FindAsync(postId, tagId);
            if (postTag != null)
            {
                _dbContext.PostTags.Remove(postTag);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<PostTag?> GetByIdsAsync(Guid postId, Guid tagId)
        {
            return await _dbContext.PostTags
                .Include(postTag => postTag.Post)
                .Include(postTag => postTag.Tag)
                .FirstOrDefaultAsync(postTag => postTag.PostId == postId && postTag.TagId == tagId);
        }

        public async Task<ICollection<PostTag>> GetByPostIdAsync(Guid postId)
        {
            return await _dbContext.PostTags
                .Include(postTag => postTag.Tag)
                .Where(postTag => postTag.PostId == postId)
                .ToListAsync();
        }

        public async Task<ICollection<PostTag>> GetByTagIdAsync(Guid tagId)
        {
            return await _dbContext.PostTags
                .Include(postTag => postTag.Post)
                .Where(postTag => postTag.TagId == tagId)
                .ToListAsync();
        }
    }
}
