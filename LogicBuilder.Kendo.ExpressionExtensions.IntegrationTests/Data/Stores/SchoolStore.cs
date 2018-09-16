using Contoso.Contexts;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Data
{
    public class SchoolStore : StoreBase, ISchoolStore
    {
        public SchoolStore(SchoolContext context)
            : base(context)
        {
        }
    }
}
