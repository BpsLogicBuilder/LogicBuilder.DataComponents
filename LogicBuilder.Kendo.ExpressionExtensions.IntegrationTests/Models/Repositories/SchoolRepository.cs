using AutoMapper;
using LogicBuilder.EntityFrameworkCore.SqlServer.Repositories;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Data;

namespace Contoso.Repositories
{
    public class SchoolRepository : ContextRepositoryBase, ISchoolRepository
    {
        public SchoolRepository(ISchoolStore store, IMapper mapper) : base(store, mapper)
        {
        }
    }
}
