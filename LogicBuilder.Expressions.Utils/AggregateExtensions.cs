using LogicBuilder.Expressions.Utils.Properties;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils
{
    public static class AggregateExtensions
    {
        /// <summary>
        /// Creates a lambda expression for an aggregate function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static LambdaExpression BuildAggregateExpression<T>(this string property, string method) where T : class
        {
            switch (method.ToLowerInvariant())
            {
                case "average":
                    return property.BuildAverageExpression<T>();
                case "count":
                    return BuildCountExpression<T>();
                case "max":
                    return property.BuildMaxExpression<T>();
                case "min":
                    return property.BuildMinExpression<T>();
                case "sum":
                    return property.BuildSumExpression<T>();
                default:
                    throw new ArgumentException(Resources.invalidMethodName);
            }
        }

        /// <summary>
        /// Returns an expression for the Count method call (no predicate) e.g. q => q.Count(). <![CDATA[ Expression<Func<IQueryable<Account>, double>> countExpression ]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, int>> BuildCountExpression<T>() where T : class
        {
            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            return Expression.Lambda<Func<IQueryable<T>, int>>(param.GetCountMethodCall(), param);
        }

        public static MethodCallExpression GetCountMethodCall(this Expression expression)
            => Expression.Call
            (
                typeof(Queryable),
                "Count",
                new Type[] { expression.GetUnderlyingElementType() },
                expression
            );

        /// <summary>
        /// Returns an expression for the Min method call which takes lambda expression as a property selctor e.g. q => q.Min(x => x.Balance). <![CDATA[ Expression<Func<IQueryable<Account>, double>> minExpression ]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static LambdaExpression BuildMinExpression<T>(this string property) where T : class
        {
            if (property == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MemberInfo propertyInfo = typeof(T).GetMemberInfoFromFullName(property);
            MethodCallExpression mce = param.GetMinMethodCall(propertyInfo, property);
            Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(IQueryable<T>), propertyInfo.GetMemberType() });

            return Expression.Lambda(delegateType, mce, param);
        }

        public static MethodCallExpression GetMinMethodCall(this Expression expression, MemberInfo memberInfo, string propertyFullName)
            => Expression.Call
            (
                typeof(Queryable), 
                "Min", 
                new Type[] { expression.GetUnderlyingElementType(), memberInfo.GetMemberType() }, 
                expression, 
                propertyFullName.GetTypedSelector(expression.GetUnderlyingElementType())
            );

        /// <summary>
        /// Returns an expression for the Max method call which takes lambda expression as a property selctor e.g. q => q.Max(x => x.Balance). <![CDATA[ Expression<Func<IQueryable<Account>, double>> maxExpression  ]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static LambdaExpression BuildMaxExpression<T>(this string property) where T : class
        {
            if (property == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MemberInfo propertyInfo = typeof(T).GetMemberInfoFromFullName(property);
            MethodCallExpression mce = param.GetMaxMethodCall(propertyInfo, property);
            Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(IQueryable<T>), propertyInfo.GetMemberType() });

            return Expression.Lambda(delegateType, mce, param);
        }

        public static MethodCallExpression GetMaxMethodCall(this Expression expression, MemberInfo memberInfo, string propertyFullName)
            => Expression.Call
            (
                typeof(Queryable),
                "Max",
                new Type[] { expression.GetUnderlyingElementType(), memberInfo.GetMemberType() },
                expression,
                propertyFullName.GetTypedSelector(expression.GetUnderlyingElementType())
            );

        /// <summary>
        /// Returns an expression for the Average method call which takes lambda expression as a property selctor e.g. q => q.Average(x => x.Balance) <![CDATA[ Expression<Func<IQueryable<Account>, double>> averageExpression ]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static LambdaExpression BuildAverageExpression<T>(this string property) where T : class
        {
            if (property == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetAverageMethodCall(property);
            Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(IQueryable<T>), typeof(T).GetMemberInfoFromFullName(property).GetMemberType() });

            return Expression.Lambda(delegateType, mce, param);
        }

        public static MethodCallExpression GetAverageMethodCall(this Expression expression, string propertyFullName)
            => Expression.Call
            (
                typeof(Queryable),
                "Average",
                new Type[] { expression.GetUnderlyingElementType() },
                expression,
                propertyFullName.GetTypedSelector(expression.GetUnderlyingElementType())
            );

        /// <summary>
        /// Returns an expression for the Sum method call which takes lambda expression as a property selctor e.g. q => q.Sum(x => x.Balance). <![CDATA[ Expression<Func<IQueryable<Account>, double>> sumExpression ]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static LambdaExpression BuildSumExpression<T>(this string property) where T : class
        {
            if (property == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetSumMethodCall(property);
            Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(IQueryable<T>), typeof(T).GetMemberInfoFromFullName(property).GetMemberType() });

            return Expression.Lambda(delegateType, mce, param);
        }

        public static MethodCallExpression GetSumMethodCall(this Expression expression, string propertyFullName)
            => Expression.Call
            (
                typeof(Queryable),
                "Sum",
                new Type[] { expression.GetUnderlyingElementType() },
                expression,
                propertyFullName.GetTypedSelector(expression.GetUnderlyingElementType())
            );

        public static Expression GetAggregateMethodCall(this Expression expression, string aggregateMethod, string propertyFullName)
        {
            switch (aggregateMethod.ToLowerInvariant())
            {
                case "average":
                    return expression.GetAverageMethodCall(propertyFullName);
                case "count":
                    return expression.GetCountMethodCall();
                case "max":
                    return expression.GetMaxMethodCall(expression.GetUnderlyingElementType().GetMemberInfoFromFullName(propertyFullName), propertyFullName);
                case "min":
                    return expression.GetMinMethodCall(expression.GetUnderlyingElementType().GetMemberInfoFromFullName(propertyFullName), propertyFullName);
                case "sum":
                    return expression.GetSumMethodCall(propertyFullName);
                default:
                    throw new ArgumentException(Resources.invalidMethodName);
            }
        }
    }
}
