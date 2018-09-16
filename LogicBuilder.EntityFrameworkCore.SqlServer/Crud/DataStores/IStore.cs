using LogicBuilder.Data;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores
{
    public interface IStore
    {
        Task<ICollection<T>> GetAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IQueryable<T>> queryFunc = null, ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null) where T : BaseData;
        Task<int> CountAsync<T>(Expression<Func<T, bool>> filter = null) where T : BaseData;
        Task<bool> SaveAsync<T>(ICollection<T> entities) where T : BaseData;
        Task<bool> SaveGraphsAsync<T>(ICollection<T> entities) where T : BaseData;
        Task<TReturn> QueryAsync<T, TReturn>(Func<IQueryable<T>, TReturn> queryableFunc, ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null) where T : BaseData;
        void AddChanges<T>(ICollection<T> entities) where T : BaseData;
        void AddGraphChanges<T>(ICollection<T> entities) where T : BaseData;
        Task<bool> SaveChangesAsync();
    }
}
