using LogicBuilder.Data;
using LogicBuilder.Domain;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Repositories
{
    public interface IModelRepository<TModel, TData>
        where TModel : BaseModel
        where TData : BaseData
    {
        Task<ICollection<TModel>> GetItemsAsync(Expression<Func<TModel, bool>> filter = null, Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null);

        Task<int> CountAsync(Expression<Func<TModel, bool>> filter = null);

        Task<TModelReturn> QueryAsync<TModelReturn, TDataReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null);

        Task<TReturn> QueryAsync<TModelReturn, TDataReturn, TReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null);

        Task<bool> SaveAsync(TModel entity);

        Task<bool> SaveAsync(ICollection<TModel> entities);

        Task<bool> SaveGraphAsync(TModel entity);

        Task<bool> SaveGraphsAsync(ICollection<TModel> entities);

        Task<bool> DeleteAsync(Expression<Func<TModel, bool>> filter = null);

        void AddChange(TModel entity);

        void AddChanges(ICollection<TModel> entities);

        void AddGraphChange(TModel entity);

        void AddGraphChanges(ICollection<TModel> entities);

        Task<bool> SaveChangesAsync();
    }
}
