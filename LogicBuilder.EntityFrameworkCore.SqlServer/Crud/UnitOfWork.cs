using LogicBuilder.Data;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DbMappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud
{
    internal class UnitOfWork : IUnitOfWork, IDisposable
    {
        internal UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        #region Variables
        private bool disposed;
        private DbContext context;
        #endregion Variables

        #region Properties
        public virtual DbContext Context
        {
            get { return this.context; }
        }

        Dictionary<Type, object> repositoryDictionary;
        public virtual Dictionary<Type, object> RepositoryDictionary
        {
            get
            {
                if (this.repositoryDictionary == null)
                    this.repositoryDictionary = new Dictionary<Type, object>();

                return this.repositoryDictionary;
            }
        }

        Dictionary<Type, object> mapperDictionary;
        public virtual Dictionary<Type, object> MapperDictionary
        {
            get
            {
                if (this.mapperDictionary == null)
                    this.mapperDictionary = new Dictionary<Type, object>();

                return this.mapperDictionary;
            }
        }
        #endregion Properties

        #region Methods
        public virtual async Task<bool> SaveChangesAsync()
        {
            return (await this.Context.SaveChangesAsync()) > 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    this.Context.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual GenericRepository<T> GetRepository<T>() where T : BaseData
        {
            if (!RepositoryDictionary.ContainsKey(typeof(T)))
                RepositoryDictionary.Add(typeof(T), new GenericRepository<T>(this.Context));

            return (GenericRepository<T>)RepositoryDictionary[typeof(T)];
        }

        public virtual DbMapperBase<T> GetMapper<T>() where T : BaseData
        {
            if (!MapperDictionary.ContainsKey(typeof(T)))
            {
                /*const string ENTITY_MAPPER_SUFFIX = "DbMapper";
                Type baseMapperType = typeof(Crud.DbMappers.DbMapperBase<>);
                string mapperTypeName = baseMapperType
                                    .AssemblyQualifiedName
                                    .Replace(baseMapperType.Name, string.Concat(typeof(T).Name, ENTITY_MAPPER_SUFFIX));*/

                /* e.g. replace "Crud.DbMappers.DbMapperBase`1, Crud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f598c2b43cddc2a9"
                 *     with "Crud.DbMappers.InstitutionDbMapper, Crud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f598c2b43cddc2a9"
                 */

                //Type mapperType = Type.GetType(mapperTypeName);
                Type mapperType = null;

                MapperDictionary.Add(typeof(T), mapperType != null
                    ? Activator.CreateInstance(mapperType, this)
                    : new DbMapperBase<T>(this));
            }

            return (DbMapperBase<T>)MapperDictionary[typeof(T)];
        }
        #endregion Methods
    }
}
