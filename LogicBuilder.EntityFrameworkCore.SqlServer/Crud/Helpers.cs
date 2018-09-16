using LogicBuilder.Data;
using LogicBuilder.EntityFrameworkCore.SqlServer.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud
{
    internal static class Helpers
    {
        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseData> entry in context.ChangeTracker.Entries<BaseData>())
                entry.State = entry.ConvertState();
        }

        public static void SetStates(this DbContext context, EntityState state)
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseData> entry in context.ChangeTracker.Entries<BaseData>())
                entry.State = state;
        }

        public static EntityState ConvertState(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseData> entry)
        {
            BaseData poco = entry.Entity;
            switch (poco.EntityState)
            {
                case EntityStateType.Added:
                    return EntityState.Added;
                case EntityStateType.Modified:
                    return EntityState.Modified;
                case EntityStateType.Deleted:
                    return EntityState.Deleted;
                case EntityStateType.Unchanged:
                    return EntityState.Unchanged;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.unKnownEntityStateFormat, Enum.GetName(typeof(EntityStateType), poco.EntityState)));
            }
        }
    }
}
