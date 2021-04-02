using Contoso.Contexts;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores;

namespace Contoso.Stores
{
    public class SchoolStore : StoreBase, ISchoolStore
    {
        public SchoolStore(SchoolContext context)
            : base(context)
        {
        }
    }
}
