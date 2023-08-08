using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used. Use LogicBuilder.Expressions.Utils.ExpressionBuilder.")]
    public static class AggregateHelpers
    {
        /// <summary>
        /// Calculates an aggregate value for the items in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aggregator"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static dynamic GetAggregateValue<T>(this Aggregator aggregator, IList<T> list) where T : class
        {
            if (list == null || list.Count == 0)
            {
                switch (aggregator.Aggregate.ToLowerInvariant())
                {
                    case "count":
                        return 0;
                    default:
                        MemberInfo propertyInfo = typeof(T).GetMemberInfoFromFullName(aggregator.Field);
                        dynamic result = propertyInfo.GetMemberType().GetTypeInfo().IsValueType ? Activator.CreateInstance(propertyInfo.GetMemberType()) : null;
                        return result ?? string.Empty;
                }
            }

            LambdaExpression aggFunc = aggregator.Field.BuildAggregateExpression<T>(aggregator.Aggregate);
            return aggFunc.Compile().DynamicInvoke(list.AsQueryable());
        }

        /// <summary>
        /// Returns a grouped collection of items as defined by the groups argument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerable Group<T>(this IEnumerable<T> items, IList<Group> groups) where T : class
        {
            if (items == null || items.Count() == 0)
                return null;

            Group group = groups.First();
            bool removed = groups.Remove(group);
            bool hasSubgroups = groups.Count() > 0;
            Func<IQueryable<T>, IQueryable<IGrouping<object, T>>> grpExp = group.Field.BuildGroupByExpression<T>().Compile();
            return grpExp(items.AsQueryable()).ToList().Aggregate(new List<object>(), (list, next) =>
            {
                list.Add(new GroupResult
                {
                    Items = removed && hasSubgroups ? Group(next.ToList(), new List<Group>(groups)) : next.ToList(),
                    Value = next.Key,
                    Aggregates = group.Aggregates.Aggregate(new Dictionary<string, object>(), (dic, ag) =>
                    {
                        List<T> grouping = next.ToList();
                        dynamic aggregateValue = ag.GetAggregateValue<T>(grouping);
                        if (dic.ContainsKey(ag.Field))
                        {
                            ((IDictionary<string, Object>)dic[ag.Field]).Add(ag.Aggregate, ag.GetAggregateValue<T>(grouping));
                        }
                        else
                        {
                            IDictionary<string, Object> aggregateObject = new ExpandoObject() as IDictionary<string, Object>;
                            aggregateObject.Add(ag.Aggregate, ag.GetAggregateValue<T>(grouping));
                            dic.Add(ag.Field, aggregateObject);
                        }

                        return dic;
                    }),
                    Count = next.Count(),
                    HasSubgroups = hasSubgroups,
                    Field = group.Field
                });
                return list;
            });
        }

        /// <summary>
        /// Returns the data source response expected by a Kendo UI Grid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="ungroupedList"></param>
        /// <param name="groupedList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static object GetDataSourceResponse<T>(this DataSourceRequestOptions request, IList<T> ungroupedList, System.Collections.IEnumerable groupedList, int count) where T : class
        {
            return new DataSourceResponse
            {
                Data = request.Group != null && request.Group.Count() > 0 ? null : ungroupedList,

                Aggregates = (request.Aggregate == null || request.Aggregate.Count() < 1) ? null : request.Aggregate.Aggregate(new Dictionary<string, object>(), (dic, ag) =>
                {
                    dynamic aggregateValue = ag.GetAggregateValue<T>(ungroupedList);
                    if (dic.ContainsKey(ag.Field))
                    {
                        ((IDictionary<string, Object>)dic[ag.Field]).Add(ag.Aggregate, ag.GetAggregateValue<T>(ungroupedList));
                    }
                    else
                    {
                        IDictionary<string, Object> aggregateObject = new ExpandoObject() as IDictionary<string, Object>;
                        aggregateObject.Add(ag.Aggregate, ag.GetAggregateValue<T>(ungroupedList));
                        dic.Add(ag.Field, aggregateObject);
                    }

                    return dic;
                }),

                Groups = request.Group != null && request.Group.Count() > 0 ? groupedList : null,
                Total = count
            };
        }
    }
}
