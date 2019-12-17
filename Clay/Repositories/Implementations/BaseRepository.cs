using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clay.Data;
using Clay.Data.Pagination;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clay.Repositories.Implementations
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IModel
    {
        private WebDbContext _ctx;

        public BaseRepository(WebDbContext context)
        {
            _ctx = context;
        }

        public virtual async Task<bool> Add(T entity)
        {
            try
            {
                _ctx.Set<T>().Add(entity);
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<PagedResult<T>> GetAll(PagedModel pagedModel)
        {
            return await _ctx.Set<T>().GetPaged(pagedModel);
        }

        public virtual async Task<PagedResult<T>> GetAll(PagedModel pagedModel, params Expression<Func<T, object>>[] includes)
        {
            var result = _ctx.Set<T>().Where(i => true);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.GetPaged(pagedModel);
        }


        public virtual async Task<PagedResult<T>> SearchBy(PagedModel pagedModel, Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes)
        {
            var result = _ctx.Set<T>().Where(searchBy);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.GetPaged(pagedModel);
        }

        public virtual async Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = _ctx.Set<T>().Where(predicate);

            foreach (var includeExpression in includes)
                result = result.Include(includeExpression);

            return await result.FirstOrDefaultAsync();
        }

        public virtual async Task<bool> Update(T entity)
        {
            try
            {
                _ctx.Set<T>().Attach(entity);
                _ctx.Entry(entity).State = EntityState.Modified;

                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> Delete(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes)
        {
            var results = _ctx.Set<T>().Where(identity);

            foreach (var includeExpression in includes)
                results = results.Include(includeExpression);
            try
            {
                _ctx.Set<T>().RemoveRange(results);
                return await Task.FromResult(true);
            }
            catch (Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public virtual async Task<bool> Delete(T entity)
        {
            _ctx.Set<T>().Remove(entity);
            return await Task.FromResult(true);
        }

    }
}