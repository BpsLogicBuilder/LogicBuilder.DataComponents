using LogicBuilder.Expressions.Utils.DataSource;
using LogicBuilder.Expressions.Utils.Strutures;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils
{
    public static class LinqHelpers
    {
        /// <summary>
        /// Returns a lambda expression whose return type is the type of the selected property e.g. person => person.FirstName as Expression<Func<T, string>>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="propertyFullName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static LambdaExpression GetTypedSelector<TSource>(this string propertyFullName, string parameterName = "a") 
            => propertyFullName.GetTypedSelector(typeof(TSource), parameterName);

        public static LambdaExpression GetTypedSelector(this string propertyFullName, Type parentType, string parameterName = "a")
        {
            ParameterExpression param = Expression.Parameter(parentType, parameterName);
            string[] parts = propertyFullName.Split('.');
            Expression parent = parts.Aggregate((Expression)param, (p, next) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(next)));

            Type[] typeArgs = new[] { parentType, parent.Type };//Generic arguments e.g. T1 and T2 MethodName<T1, T2>(method arguments)
            Type delegateType = typeof(Func<,>).MakeGenericType(typeArgs);//Delegate type for the selector expression.  It takes a TSource and returns the sort property type
            return Expression.Lambda(delegateType, parent, param);//Resulting lambda expression for the selector.
        }

        /// <summary>
        /// Returns a lambda expression whose return type is object e.g. person => person.FirstName as Expression<Func<T, object>>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="propertyFullName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static LambdaExpression GetObjectSelector<TSource>(this string propertyFullName, string parameterName = "a") 
            => propertyFullName.GetObjectSelector(typeof(TSource), parameterName);

        public static LambdaExpression GetObjectSelector(this string propertyFullName, Type parentType, string parameterName = "a")
        {
            ParameterExpression param = Expression.Parameter(parentType, parameterName);
            string[] parts = propertyFullName.Split('.');
            Expression parent = parts.Aggregate((Expression)param, (p, next) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(next)));

            if (parent.Type.GetTypeInfo().IsValueType)//Convert value type expressions to object expressions otherwise
                parent = Expression.Convert(parent, typeof(object));//Expression.Lambda below will throw an exception for value types

            Type[] typeArgs = new[] { parentType, typeof(object) };//Generic arguments e.g. T1 and T2 MethodName<T1, T2>(method arguments)
            Type delegateType = typeof(Func<,>).MakeGenericType(typeArgs);//Delegate type for the selector expression.  It takes a TSource and returns typeof(object) (the sort property type could string, any value type or a nullable of a value type)
            return Expression.Lambda(delegateType, parent, param);//Resulting lambda expression for the selector.
        }

        /// <summary>
        /// Returns the full name of property given the member expression.
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public static string GetFullName(this MemberExpression memberExpression)
        {
            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.Parameter:
                    return memberExpression.Member.Name;
                case ExpressionType.MemberAccess:
                    return string.Concat(((MemberExpression)memberExpression.Expression).GetFullName(), ".", memberExpression.Member.Name);
                default:
                    throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unexpected expression node type. {0}", memberExpression.Expression.NodeType));
            }
        }

        /// <summary>
        /// Returns a member expression created from the full name of the property. 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="propertyFullName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MemberExpression GetMemberExpression<TSource>(this string propertyFullName, string parameterName = "a")
        {
            ParameterExpression param = Expression.Parameter(typeof(TSource), parameterName);
            string[] parts = propertyFullName.Split('.');
            Expression parent = parts.Aggregate((Expression)param, (p, next) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(next)));
            return (MemberExpression)parent;
        }

        /// <summary>
        /// ToS elector
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="include"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, TReturn>> ToSelector<TSource, TReturn>(Expression<Func<TSource, TReturn>> include)
            => include;

        /// <summary>
        /// To Filter
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, bool>> ToFilter<TSource>(Expression<Func<TSource, bool>> filter)
            => filter;

        /// <summary>
        /// To Expression
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<TDelegate> ToExpression<TDelegate>(Expression<TDelegate> expression)
            => expression;

        public static FilteredIncludeExpression BuildFilteredIncludeExpression(this FilteredInclude filteredInclude, Type type)
        {
            LambdaExpression include = QueryExtensions.BuildSelectorExpression(type, filteredInclude.Include);

            Type propertyType = (include.Body as MemberExpression).GetMemberType().GetCurrentType();
            return new FilteredIncludeExpression
            {
                Include = include,
                Filter = filteredInclude.FilterGroup.GetFilterExpression(propertyType),
                FilteredIncludes = filteredInclude.FilteredIncludes?.Select(fi => fi.BuildFilteredIncludeExpression(propertyType)).ToList()
            };
        }
    }
}
