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
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _dbContext;

        public PostRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Post post)
        {
            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            if (post != null)
            {
                _dbContext.Posts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Post>> GetAllAsync()
        {
            return await _dbContext.Posts
                .Include(post => post.User)
                .Include(post => post.Topic)
                .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Posts
                .Include(post => post.User)
                .Include(post => post.Topic)
                .Include(post => post.Comments)
                .Include(post => post.Votes)
                .FirstOrDefaultAsync(post => post.Id == id);
        }

        public async Task UpdateAsync(Post post)
        {
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
        }
    }
}
