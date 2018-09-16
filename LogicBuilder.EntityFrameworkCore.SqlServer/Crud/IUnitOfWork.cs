using LogicBuilder.Data;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DbMappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud
{
    internal interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        Dictionary<Type, object> RepositoryDictionary { get; }
        Dictionary<Type, object> MapperDictionary { get; }
        Task<bool> SaveChangesAsync();
        GenericRepository<T> GetRepository<T>() where T : BaseData;
        DbMapperBase<T> GetMapper<T>() where T : BaseData;
    }
}
