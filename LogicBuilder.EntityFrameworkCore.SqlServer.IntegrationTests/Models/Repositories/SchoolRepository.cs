using AutoMapper;
using Contoso.Stores;
using LogicBuilder.EntityFrameworkCore.SqlServer.Repositories;

namespace Contoso.Repositories
{
    public class SchoolRepository : ContextRepositoryBase, ISchoolRepository
    {
        public SchoolRepository(ISchoolStore store, IMapper mapper) : base(store, mapper)
        {
        }
    }
}
