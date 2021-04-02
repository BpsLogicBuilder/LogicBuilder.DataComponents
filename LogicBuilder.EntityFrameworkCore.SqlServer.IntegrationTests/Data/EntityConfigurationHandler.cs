using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Contoso.Contexts
{
    public class EntityConfigurationHandler
    {
        public EntityConfigurationHandler(DbContext context)
        {
            this.Context = context;
        }

        #region Properties
        protected DbContext Context { get; private set; }
        #endregion Properties

        #region Methods
        public virtual void Configure(ModelBuilder modelBuilder)
        {
            HashSet<string> mapNames = this.Context.GetType().GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => string.Concat(p.PropertyType.GetGenericArguments()[0].Name, "Configuration"))
                .Aggregate(new HashSet<string>(), (hashSet, next) =>
                {
                    if (!hashSet.Contains(next))
                        hashSet.Add(next);

                    return hashSet;
                });

            Type interfaceType = typeof(Configuations.ITableConfiguration);
            interfaceType.GetTypeInfo().Assembly.GetTypes().Where(p => interfaceType.IsAssignableFrom(p)
                                && mapNames.Contains(p.Name)
                                && !p.GetTypeInfo().IsAbstract
                                && !p.GetTypeInfo().IsGenericTypeDefinition
                                && !p.GetTypeInfo().IsInterface).ToList().ForEach(t =>
                                {
                                    MethodInfo mi = t.GetMethod("Configure");
                                    mi.Invoke(Activator.CreateInstance(t), new object[] { modelBuilder });
                                });
        }
        #endregion Methods
    }
}
