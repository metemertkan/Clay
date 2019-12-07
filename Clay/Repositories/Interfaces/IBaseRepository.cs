using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Clay.Data.Pagination;

namespace Clay.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<bool> Add(T entity);

        Task<PagedResult<T>> GetAll(PagedModel pagedModel);

        Task<PagedResult<T>> GetAll(PagedModel pagedModel, params Expression<Func<T, object>>[] includes);

        Task<PagedResult<T>> SearchBy(PagedModel pagedModel, Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes);

        Task<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<bool> Update(T entity);

        Task<bool> Delete(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes);

        Task<bool> Delete(T entity);

    }
}