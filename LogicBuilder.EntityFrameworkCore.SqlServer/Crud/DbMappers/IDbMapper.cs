using System.Collections.Generic;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DbMappers
{
    internal interface IDbMapper<T>
    {
        void AddChanges(ICollection<T> entities);
        void AddGraphChanges(ICollection<T> entities);
    }
}
