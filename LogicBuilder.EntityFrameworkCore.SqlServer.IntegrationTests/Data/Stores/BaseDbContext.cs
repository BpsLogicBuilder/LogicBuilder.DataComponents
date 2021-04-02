using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso.Contexts
{
    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
            this.EntityConfigurationHandler = new EntityConfigurationHandler(this);
        }

        protected virtual EntityConfigurationHandler EntityConfigurationHandler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.EntityConfigurationHandler.Configure(modelBuilder);
        }
    }
}
