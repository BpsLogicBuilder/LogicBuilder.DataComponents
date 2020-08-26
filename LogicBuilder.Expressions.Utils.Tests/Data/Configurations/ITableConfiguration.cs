using Microsoft.EntityFrameworkCore;

namespace Contoso.Contexts.Configuations
{
    interface ITableConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
