using LogicBuilder.Data;
using LogicBuilder.EntityFrameworkCore.SqlServer.Properties;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Conversions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud
{
    internal static class Helpers
    {
        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (EntityEntry<BaseData> entry in context.ChangeTracker.Entries<BaseData>())
                entry.State = entry.ConvertState();
        }

        public static void SetStates(this DbContext context, EntityState state)
        {
            foreach (EntityEntry<BaseData> entry in context.ChangeTracker.Entries<BaseData>())
                entry.State = state;
        }

        public static EntityState ConvertState(this EntityEntry<BaseData> entry)
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

        public static void DetachMatchingKeyEntries(this DbContext context, object sourceEntity)
        {
            List<BaseData> entities = new List<BaseData>();
            GetEntityList((BaseData)sourceEntity, entities);

            entities.ForEach
            (
                entity => 
                {
                    Type eType = entity.GetType();
                    var entry = context.Entry(entity);

                    entry.Metadata
                        .FindPrimaryKey()
                        .Properties
                        .ToDictionary
                        (
                            p => p.Name, 
                            p => entry.Property(p.Name).CurrentValue
                        )
                        .Aggregate
                        (
                            context.ChangeTracker
                                .Entries()
                                .Where(e => e.Entity.GetType() == eType)
                                .AsQueryable(), 
                            (query, next) => query.Where(GetPrimaryKeyFilter(eType, next.Key, next.Value))
                        )
                        .ToList()
                        .ForEach(item => item.State = EntityState.Detached);
                }
            );
        }

        private static void GetEntityList(System.Collections.IEnumerable source, List<BaseData> entities)
        {
            System.Collections.IEnumerator enumerator = source.GetEnumerator();
            while (true)
            {
                if (!enumerator.MoveNext()) break;
                if (enumerator.Current == null) continue;
                GetEntityList(enumerator.Current, enumerator.Current.GetType(), entities);
            }
        }

        private static void GetEntityList(BaseData entity, List<BaseData> entities)
        {
            entities.Add(entity);

            foreach (PropertyInfo pInfo in entity.GetType().GetProperties())
                GetEntityList(pInfo.GetValue(entity, null), pInfo.PropertyType, entities);
        }

        private static void GetEntityList(object entity, Type entityType, List<BaseData> entities)
        {
            if (entity == null)
                return;

            if (entityType.IsLiteralType())
                return;
            else if (typeof(BaseData).IsAssignableFrom(entityType))
                GetEntityList((BaseData)entity, entities);
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(entityType))
                GetEntityList((System.Collections.IEnumerable)entity, entities);
        }

        private static Expression<Func<EntityEntry, bool>> GetPrimaryKeyFilter(Type entityType, string propertyName, object propertyValue) 
            => (Expression<Func<EntityEntry, bool>>)GetFilterLambdaOperator(entityType, propertyName, propertyValue).Build();

        private static FilterLambdaOperator GetFilterLambdaOperator(Type entityType, string propertyName, object propertyValue)
        {
            var parameters = new Dictionary<string, ParameterExpression>();
            string parameterName = "f";

            return new FilterLambdaOperator
            (
                parameters,
                new EqualsBinaryOperator
                (
                    new MemberSelectorOperator
                    (
                        propertyName,
                        new CastOperator
                        (
                            new MemberSelectorOperator("Entity", new ParameterOperator(parameters, parameterName)),
                            entityType
                        )
                    ),
                    new ConstantOperator(propertyValue, propertyValue.GetType())
                ),
                typeof(EntityEntry),
                parameterName
            );
        }
    }
}
