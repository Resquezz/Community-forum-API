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
    public class VoteRepository : IVoteRepository
    {
        private readonly AppDbContext _dbContext;

        public VoteRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Vote vote)
        {
            await _dbContext.Votes.AddAsync(vote);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var vote = await _dbContext.Votes.FindAsync(id);
            if (vote != null)
            {
                _dbContext.Votes.Remove(vote);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Vote>> GetAllAsync()
        {
            return await _dbContext.Votes
                .Include(vote => vote.User)
                .Include(vote => vote.Post)
                .Include(vote => vote.Comment)
                .ToListAsync();
        }

        public async Task<Vote?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Votes
                .Include(vote => vote.User)
                .Include(vote => vote.Post)
                .Include(vote => vote.Comment)
                .FirstOrDefaultAsync(vote => vote.Id == id);
        }

        public async Task UpdateAsync(Vote vote)
        {
            _dbContext.Votes.Update(vote);
            await _dbContext.SaveChangesAsync();
        }
    }
}
