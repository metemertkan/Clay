using Clay.Data.Pagination;
using Clay.Repositories.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clay.Models.Domain;
using Microsoft.Extensions.Caching.Distributed;

namespace Clay.Repositories.Cached
{
    public class CachedBaseRepositoryDecorator<T> : IBaseRepository<T> where T : class, IModel
    {
        private readonly IBaseRepository<T> _baseRepository;
        private readonly IDistributedCache _distributedCache;

        public CachedBaseRepositoryDecorator(IBaseRepository<T> baseRepository, IDistributedCache distributedCache)
        {
            _baseRepository = baseRepository;
            _distributedCache = distributedCache;
        }

        public Task<bool> Add(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<T>> GetAll(PagedModel pagedModel)
        {
            var stringAsync = _distributedCache.GetStringAsync("");
            throw new NotImplementedException();
        }

        public Task<PagedResult<T>> GetAll(PagedModel pagedModel, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<T>> SearchBy(PagedModel pagedModel, Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}