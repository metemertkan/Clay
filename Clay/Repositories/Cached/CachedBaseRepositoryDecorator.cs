using Clay.Data.Pagination;
using Clay.Repositories.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clay.Helpers;
using Clay.Models.Domain;
using Clay.Models.Domain.Base;
using Microsoft.Extensions.Caching.Distributed;

namespace Clay.Repositories.Cached
{
    public class CachedBaseRepositoryDecorator<T> : IBaseRepository<T> where T : class, IModel
    {
        private readonly IBaseRepository<T> _baseRepository;
        private readonly IDistributedCache _distributedCache;
        private DistributedCacheEntryOptions _cacheEntryOptions;

        public CachedBaseRepositoryDecorator(IBaseRepository<T> baseRepository, IDistributedCache distributedCache)
        {
            _baseRepository = baseRepository;
            _distributedCache = distributedCache;
            _cacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.MaxValue };
        }

        public async Task<bool> Add(T entity)
        {
            await InvalidateCaches(typeof(T).Name);
            return await _baseRepository.Add(entity);
        }

        public async Task<bool> Delete(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes)
        {
            await InvalidateCaches(typeof(T).Name);
            return await _baseRepository.Delete(identity, includes);
        }

        public async Task<bool> Delete(T entity)
        {
            await InvalidateCaches(typeof(T).Name);
            return await _baseRepository.Delete(entity);
        }

        public async Task<PagedResult<T>> GetAll(PagedModel pagedModel)
        {
            var generatedKey =
                CacheKeyHelper.GenerateKeyWithPagination(typeof(T).Name, pagedModel.Page, pagedModel.PageSize);

            var cachedData = await _distributedCache.GetAsync<PagedResult<T>>(generatedKey);
            if (cachedData != null && cachedData.Results.Count != 0)
                return cachedData;

            var dbData = await _baseRepository.GetAll(pagedModel);
            await _distributedCache.SetAsync<PagedResult<T>>(generatedKey, dbData, _cacheEntryOptions);
            return dbData;
        }

        public async Task<PagedResult<T>> GetAll(PagedModel pagedModel, params Expression<Func<T, object>>[] includes)
        {
            var dbData = await _baseRepository.GetAll(pagedModel, includes);
            return dbData;
        }

        public async Task<PagedResult<T>> SearchBy(PagedModel pagedModel, Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes)
        {
            var dbData = await _baseRepository.SearchBy(pagedModel, searchBy, includes);
            return dbData;
        }

        public async Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var dbData = await _baseRepository.FindBy(predicate, includes);
            return dbData;
        }

        public async Task<bool> Update(T entity)
        {
            await InvalidateCaches(typeof(T).Name);
            return await _baseRepository.Update(entity);
        }

        private async Task InvalidateCaches(string startsWith)
        {
            var generatedKeys = CacheKeyHelper.GetKeysStartsWith(startsWith);
            await _distributedCache.InvalidateKeysAsync(generatedKeys);
        }
    }
}