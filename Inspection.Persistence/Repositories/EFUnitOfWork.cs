
using Inspection.Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly InspectionDbContext _dbContext;
        private IDbContextTransaction? _currentTx;

        public EFUnitOfWork(InspectionDbContext dbContext)
        => _dbContext = dbContext;


        public async Task BeginTransactionAsync()
        => _currentTx = await _dbContext.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            await _dbContext.SaveChangesAsync();
            if (_currentTx != null)
                await _currentTx.CommitAsync();
        }

        public async Task RollBackAsync()
        {
            if (_currentTx != null)
                await _currentTx.RollbackAsync();
        }
    }
}
