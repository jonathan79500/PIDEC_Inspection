using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspection.Application.Contracts.Persistence;

public interface IGenericRepository<T , IdType> where T:class where IdType : class
{
    Task<T> Get(IdType id);
    Task<IReadOnlyList<T>> GetAll();
    Task Delete(T entity);
    Task<T> Update(T entity);
    Task<T> Add(T entity);


}
