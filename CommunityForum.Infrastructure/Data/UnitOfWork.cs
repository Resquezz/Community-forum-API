using CommunityForum.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityForum.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await _currentTransaction.RollbackAsync();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if(_currentTransaction is not null)
            {
                await _currentTransaction.RollbackAsync();
                _currentTransaction.Dispose();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
