using Inspection.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence.Repositories;

public class GenericRepository<T, IdType> : IGenericRepository<T,IdType> where T : class where IdType : class
{

    private readonly InspectionDbContext _dbContext;

    public GenericRepository(InspectionDbContext dbContext)
    {

        _dbContext = dbContext;

    }

    public async Task<T> Add(T entity)
    {
        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T> Get(IdType id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAll()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T> Update(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
         await _dbContext.SaveChangesAsync();
        return entity;
    }

}
