using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthrisationApi.Models;

namespace AuthrisationApi.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseModel, new()
    {
        IList<TEntity> GetAll();

        IList<TEntity> GetAllWithInclude(params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetAsync(string id);

        Task<TEntity> GetAsyncWithInclude(string id, Expression<Func<TEntity, object>>[] includes);

        Task<string> InsertAsync(TEntity entity);

        Task<string> UpdateAsync(TEntity entity);

        Task DeleteAsync(string id);

        Task DeleteAsync(TEntity entity);

        IList<TEntity> Search(Expression<Func<TEntity, bool>> where);

        IList<TEntity> SearchWithInclude(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
    }
}