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
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _dbContext;

        public CommentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Comment comment)
        {
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Comment>> GetAllAsync()
        {
            return await _dbContext.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.Post)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.Post)
                .Include(comment => comment.Replies)
                .Include(comment => comment.Votes)
                .FirstOrDefaultAsync(comment => comment.Id == id);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
