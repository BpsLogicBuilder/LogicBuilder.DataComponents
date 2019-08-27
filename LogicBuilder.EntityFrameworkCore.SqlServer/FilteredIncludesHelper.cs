using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions.Impl;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LogicBuilder.EntityFrameworkCore.SqlServer
{
    internal static class FilteredIncludesHelper
    {
        #region Constants
        private const string LOAD_COLLECTION_METHOD = "DoLoadCollection";
        private const string LOAD_REFERENCE_METHOD = "DoLoadReference";
        private const string MAP_FILTERED_INCLUDES_METHOD = "MapFilteredIncludes";
        #endregion Constants

        #region Methods
        internal static void DoExplicitLoading<T>(DbContext context, List<T> list, ICollection<FilteredIncludeExpression> filteredIncludes) where T : class
        {
            if (filteredIncludes == null || filteredIncludes.Count == 0)
                return;

            filteredIncludes.ToList().ForEach(exp =>
            {
                context.InvokeMethodForCollection
                (
                    list,
                    exp.Include.Body.GetMemberExpression().GetMemberType(),
                    exp
                );
            });
        }

        private static void InvokeMethodForCollection<T>(this DbContext context, List<T> list, Type memberType, FilteredIncludeExpression filteredIncludes) where T : class
        {
            if (memberType.IsList())
            {
                list.ForEach(item =>
                {
                    LOAD_COLLECTION_METHOD.GetMethod().MakeGenericMethod
                    (
                        typeof(T),
                        memberType.GetUnderlyingElementType()
                    ).Invoke(null, new object[] { context, context.GetEntry(item), filteredIncludes });
                });
            }
            else
            {
                list.ForEach(item =>
                {
                    LOAD_REFERENCE_METHOD.GetMethod().MakeGenericMethod
                    (
                        typeof(T),
                        memberType
                    ).Invoke(null, new object[] { context, context.GetEntry(item), filteredIncludes });
                });
            }
        }

        private static void InvokeMethodForReference<T>(this DbContext context, T property, Type memberType, FilteredIncludeExpression filteredIncludes) where T : class
        {
            if (property == null)
                return;

            if (memberType.IsList())
            {
                LOAD_COLLECTION_METHOD.GetMethod().MakeGenericMethod
                (
                    typeof(T),
                    memberType.GetUnderlyingElementType()
                ).Invoke(null, new object[] { context, context.GetEntry(property), filteredIncludes });
            }
            else
            {
                LOAD_REFERENCE_METHOD.GetMethod().MakeGenericMethod
                (
                    typeof(T),
                    memberType
                ).Invoke(null, new object[] { context, context.GetEntry(property), filteredIncludes });
            }
        }

        private static void DoLoadCollection<T, TProperty>(DbContext context, EntityEntry<T> entry, FilteredIncludeExpression filteredInclude) where T : class where TProperty : class
        {
            List<TProperty> collection = filteredInclude.Filter != null
                ? entry.Collection((Expression<Func<T, IEnumerable<TProperty>>>)filteredInclude.Include)
                        .Query()
                        .Where((Expression<Func<TProperty, bool>>)filteredInclude.Filter).ToList()
                : entry.Collection((Expression<Func<T, IEnumerable<TProperty>>>)filteredInclude.Include)
                        .Query()
                        .ToList();

            if (filteredInclude.FilteredIncludes == null || filteredInclude.FilteredIncludes.Count == 0)
                return;

            filteredInclude.FilteredIncludes.ToList().ForEach(exp =>
            {
                context.InvokeMethodForCollection
                (
                    collection,
                    exp.Include.Body.GetMemberExpression().GetMemberType(),
                    exp
                );
            });
        }

        private static void DoLoadReference<T, TProperty>(DbContext context, EntityEntry<T> entry, FilteredIncludeExpression filteredInclude) where T : class where TProperty : class
        {
            TProperty property = entry.Reference((Expression<Func<T, TProperty>>)filteredInclude.Include)
                                        .Query()
                                        .SingleOrDefault((Expression<Func<TProperty, bool>>)filteredInclude.Filter);

            if (filteredInclude.FilteredIncludes == null || filteredInclude.FilteredIncludes.Count == 0)
                return;

            filteredInclude.FilteredIncludes.ToList().ForEach(exp =>
            {
                context.InvokeMethodForReference<TProperty>
                (
                    property,
                    exp.Include.Body.GetMemberExpression().GetMemberType(),
                    exp
                );
            });
        }

        private static MethodInfo GetMethod(this string methodName)
            => typeof(FilteredIncludesHelper).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

        private static EntityEntry<T> GetEntry<T>(this DbContext context, T item) where T : class
        {
            return GetAttachedEntry(context.Entry(item));

            EntityEntry<T> GetAttachedEntry(EntityEntry<T> entry)
            {
                if (entry.State == EntityState.Detached)
                    context.Set<T>().Attach(item);//DbContext must be transient

                return entry;
            }
        }

        public static MemberExpression GetMemberExpression(this Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return (expr as UnaryExpression)?.Operand as MemberExpression;
                default:
                    return expr as MemberExpression;
            }
        }

        internal static ICollection<FilteredIncludeExpression> MapFilteredIncludes<TModel, TData>(this ICollection<FilteredIncludeExpression> filteredIncludes, IMapper mapper)
        {
            if (filteredIncludes == null)
                return null;

            LambdaExpression GetFilter(LambdaExpression nextFilter, Type sourceMemberType, Type destMemberType)
            {
                if (nextFilter == null)
                    return null;

                return (LambdaExpression)mapper.Map
                (
                    nextFilter,
                    typeof(Expression<>).MakeGenericType
                    (
                        new Type[]
                        {
                            sourceMemberType.IsList()
                                ? typeof(Func<,>).MakeGenericType(new Type[] { sourceMemberType.GetUnderlyingElementType(), typeof(bool) })
                                : typeof(Func<,>).MakeGenericType(new Type[] { sourceMemberType, typeof(bool) })
                        }
                    ),
                    typeof(Expression<>).MakeGenericType
                    (
                        new Type[] 
                        {
                            destMemberType.IsList()
                                ? typeof(Func<,>).MakeGenericType(new Type[] { destMemberType.GetUnderlyingElementType(), typeof(bool) })
                                : typeof(Func<,>).MakeGenericType(new Type[] { destMemberType, typeof(bool) })
                        }
                    )
                );
            }

            return filteredIncludes.Aggregate(new List<FilteredIncludeExpression>(), (list, next) =>
            {
                if (next.Include == null)
                    throw new ArgumentNullException("FilteredIncludeExpression.Include is required.");

                //Map with member converted to object
                LambdaExpression mappedInclude = mapper.MapExpressionAsInclude<Expression<Func<TData, object>>>(next.Include);

                //Get the member expression as an "uncast" member expression
                MemberExpression mappedIncludeMemberExpression = mappedInclude.Body.GetMemberExpression();

                //Uncast expression's member type for the selected member
                Type destMemberType = mappedIncludeMemberExpression.GetMemberType();

                //Uncast expression's member type for the source's selected member
                Type sourceMemberType = next.Include.Body.GetMemberExpression().GetMemberType();

                //Recreate the lanbda expression so it is strongly typed.  For collections the type must be IEnumerable<TProperty>.
                //EntityEntry<T>.Collection expects an expression selector of type Expression<Func<T, IEnumerable<TProperty>>> 
                mappedInclude = Expression.Lambda
                (
                    typeof(Func<,>).MakeGenericType
                    (
                        new Type[]
                        {
                            typeof(TData),
                            destMemberType.IsList()
                                ? typeof(IEnumerable<>).MakeGenericType(new Type[] { destMemberType.GetUnderlyingElementType() })
                                : destMemberType
                        }
                    ),
                    mappedIncludeMemberExpression,
                    mappedInclude.Parameters[0]
                );


                list.Add
                (
                    new FilteredIncludeExpression
                    {
                        Include = mappedInclude,
                        Filter = GetFilter(next.Filter, sourceMemberType, destMemberType),
                        FilteredIncludes = next.FilteredIncludes == null ? null : (ICollection<FilteredIncludeExpression>)MAP_FILTERED_INCLUDES_METHOD.GetMethod().MakeGenericMethod
                        (
                            /*TModel*/sourceMemberType.IsList() ? sourceMemberType.GetUnderlyingElementType() : sourceMemberType,//should not be a literal type.  A flattened include should not have child includes
                            /*TData*/destMemberType.IsList() ? destMemberType.GetUnderlyingElementType() : destMemberType
                        ).Invoke(null, new object[] { next.FilteredIncludes, mapper })
                    }
                );
                return list;
            });
        }
        #endregion Methods
    }
}
