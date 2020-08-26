using LogicBuilder.Expressions.Utils.DataSource;
using LogicBuilder.Expressions.Utils.FilterBuilder;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.OData.Edm;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils
{
    public static class LinqHelpers
    {
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

        public static MemberExpression GetMemberExpression<TSource>(this string propertyFullName, string parameterName = "a")
        {
            ParameterExpression param = Expression.Parameter(typeof(TSource), parameterName);
            string[] parts = propertyFullName.Split('.');
            Expression parent = parts.Aggregate((Expression)param, (p, next) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(next)));
            return (MemberExpression)parent;
        }

        public static Expression<Func<TSource, TReturn>> ToSelector<TSource, TReturn>(Expression<Func<TSource, TReturn>> include)
            => include;

        public static Expression<Func<TSource, bool>> ToFilter<TSource>(Expression<Func<TSource, bool>> filter)
            => filter;

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

        public static Expression<Func<T, bool>> GetFilter<T>(this FilterFunctionGroup functionGroup, ParameterExpression parameter)
            => (Expression<Func<T, bool>>)functionGroup.GetFilter(parameter);

        public static LambdaExpression GetFilter(this FilterFunctionGroup functionGroup, ParameterExpression parameter)
            => Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType(parameter.Type, typeof(bool)),
                functionGroup.Build(),
                parameter
            );

        public static Expression<Func<T, bool>> GetFilter<T>(this IExpressionPart filterPart, ParameterExpression parameter)
            => (Expression<Func<T, bool>>)filterPart.GetFilter(parameter);

        public static LambdaExpression GetFilter(this IExpressionPart filterPart, ParameterExpression parameter)
        {
            return Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType(parameter.Type, typeof(bool)),
                ConvertBody(filterPart.Build()),
                parameter
            );

            static Expression ConvertBody(Expression body)
            {
                if (body.Type != typeof(bool))
                    return Expression.Convert(body, typeof(bool));

                return body;
            }
        }

        public static Expression MakeValueSelectorAccessIfNullable(this Expression expression)
        {
            if (!expression.Type.IsNullableType())
                return expression;

            return expression.MakeSelector("Value");
        }

        public static Expression MakeHasValueSelector(this Expression expression)
        {
            if (!expression.Type.IsNullableType())
                throw new ArgumentException(nameof(expression));

            return expression.MakeSelector("HasValue");
        }

        public static Expression MakeSelector(this Expression expression, string memberFullName)
            => memberFullName.Split('.')
                .Aggregate
                (
                    expression,
                    (ex, next) => Expression.MakeMemberAccess
                    (
                        ex,
                        ex.Type.GetMemberInfo(next)
                    )
                );

        public static Expression SetNullType(this Expression expression, Type type)
        {
            if (expression is ConstantExpression constantExpression && constantExpression.Value == null)
                return Expression.Constant(null, type);

            return expression;
        }

        public static Expression MakeDaySelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTime)
                            || operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(Date))
                return operandExpression.MakeSelector("Day");
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeMonthSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTime)
                            || operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(Date))
                return operandExpression.MakeSelector("Month");
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeYearSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTime)
                            || operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(Date))
                return operandExpression.MakeSelector("Year");
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeHourSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(DateTime))
            {
                return operandExpression.MakeSelector("Hour");
            }
            else if (operandExpression.Type == typeof(TimeSpan)
                || operandExpression.Type == typeof(TimeOfDay))
            {
                return operandExpression.MakeSelector("Hours");
            }
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeMinuteSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(DateTime))
            {
                return operandExpression.MakeSelector("Minute");
            }
            else if (operandExpression.Type == typeof(TimeSpan)
                || operandExpression.Type == typeof(TimeOfDay))
            {
                return operandExpression.MakeSelector("Minutes");
            }
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeSecondSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(DateTime))
            {
                return operandExpression.MakeSelector("Second");
            }
            else if (operandExpression.Type == typeof(TimeSpan)
                || operandExpression.Type == typeof(TimeOfDay))
            {
                return operandExpression.MakeSelector("Seconds");
            }
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeMillisecondSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTimeOffset)
                            || operandExpression.Type == typeof(DateTime))
            {
                return operandExpression.MakeSelector("Millisecond");
            }
            else if (operandExpression.Type == typeof(TimeSpan)
                || operandExpression.Type == typeof(TimeOfDay))
            {
                return operandExpression.MakeSelector("Milliseconds");
            }
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeDateSelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTime)
                || operandExpression.Type == typeof(DateTimeOffset))
                return operandExpression.MakeSelector("Date");
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression MakeTimeOfDaySelector(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(DateTime)
                || operandExpression.Type == typeof(DateTimeOffset))
                return operandExpression.MakeSelector("TimeOfDay");
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression GetEnumerableCountCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                typeof(Enumerable),
                "Count",
                new[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static Expression GetEnumerableContainsCall(this Expression expression, Expression operand)
            => Expression.Call
            (
                typeof(Enumerable),
                "Contains",
                new[] { expression.GetUnderlyingElementType() },
                expression,
                operand
            );

        public static Expression GetStringContainsCall(this Expression instance, Expression operand)
            => Expression.Call(instance, StringContainsMethodInfo, operand);

        public static Expression GetEnumerableConcatCall(this Expression expression, Expression operand)
            => Expression.Call
            (
                typeof(Enumerable),
                "Concat",
                new[] { expression.GetUnderlyingElementType() },
                expression,
                operand
            );

        public static MethodCallExpression GetAnyCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                expression.Type.IsIQueryable() ? typeof(Queryable) : typeof(Enumerable),
                "Any",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetAllCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                expression.Type.IsIQueryable() ? typeof(Queryable) : typeof(Enumerable),
                "All",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetAnyEnumerableCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                typeof(Enumerable),
                "Any",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetAllEnumerableCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                typeof(Enumerable),
                "All",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetAnyQueryableCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                typeof(Queryable),
                "Any",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetAllQueryableCall(this Expression expression, params Expression[] args)
            => Expression.Call
            (
                typeof(Queryable),
                "All",
                new Type[] { expression.GetUnderlyingElementType() },
                new Expression[] { expression }.Concat(args).ToArray()
            );

        public static MethodCallExpression GetOfTypeEnumerableCall(this Expression expression, Type elementType)
           => Expression.Call
           (
               typeof(Enumerable),
               "OfType",
               new Type[] { elementType },
               expression
           );

        public static MethodCallExpression GetOfTypeQueryableCall(this Expression expression, Type elementType)
            => Expression.Call
            (
                typeof(Queryable),
                "OfType",
                new Type[] { elementType },
                expression
            );

        public static Expression GetStringConcatCall(Expression left, Expression right)
            => Expression.Call(StringConcatMethodInfo, left, right);

        public static Expression GetHasFlagCall(this Expression instance, Expression operand)
            => Expression.Call(instance, EnumHasFlagMethodInfo, operand);

        public static Expression GetStringEndsWithCall(this Expression instance, Expression operand)
            => Expression.Call(instance, StringEndsWithMethodInfo, operand);

        public static Expression GetStringStartsWithCall(this Expression instance, Expression operand)
            => Expression.Call(instance, StringStartsWithMethodInfo, operand);

        public static Expression GetStringIndexOfCall(this Expression instance, Expression operand)
            => Expression.Call(instance, StringIndexOfMethodInfo, operand);

        public static Expression GetSubStringCall(this Expression instance, params Expression[] args)
        {
            return args.Length switch
            {
                1 => Expression.Call(instance, StringSubstringStartMethodInfo, args),
                2 => Expression.Call(instance, StringSubstringStartFinishMethodInfo, args),
                _ => throw new ArgumentException(nameof(args)),
            };
        }

        public static Expression GetStringToLowerCall(this Expression instance)
            => Expression.Call(instance, StringToLowerMethodInfo);

        public static Expression GetStringToUpperCall(this Expression instance)
            => Expression.Call(instance, StringToUpperMethodInfo);

        public static Expression GetStringTrimCall(this Expression instance)
            => Expression.Call(instance, StringTrimMethodInfo);

        public static Expression GetObjectToStringCall(this Expression instance)
            => Expression.Call(instance, ToStringMethodInfo);

        public static Expression GetMaxDateTimOffsetField()
            => Expression.MakeMemberAccess(null, DateTimeMaxMemberInfo);

        public static Expression GetMinDateTimOffsetField()
            => Expression.MakeMemberAccess(null, DateTimeMinMemberInfo);

        public static Expression GetNowDateTimOffsetProperty()
            => Expression.MakeMemberAccess(null, DateTimeUtcNowMemberInfo);

        public static Expression GetCeilingCall(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(decimal))
                return operandExpression.GetDecimalCeilingCall();
            else if (operandExpression.Type == typeof(double))
                return operandExpression.GetDoubleCeilingCall();
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression GetDecimalCeilingCall(this Expression operand)
            => Expression.Call(DecimalCeilingMethodInfo, operand);

        public static Expression GetDoubleCeilingCall(this Expression operand)
           => Expression.Call(DoubleCeilingMethodInfo, operand);

        public static Expression GetRoundCall(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(decimal))
                return operandExpression.GetDecimalRoundCall();
            else if (operandExpression.Type == typeof(double))
                return operandExpression.GetDoubleRoundCall();
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression GetDecimalRoundCall(this Expression operand)
            => Expression.Call(DecimalRoundMethodInfo, operand);

        public static Expression GetDoubleRoundCall(this Expression operand)
           => Expression.Call(DoubleRoundMethodInfo, operand);

        public static Expression GetFloorCall(this Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(decimal))
                return operandExpression.GetDecimalFloorCall();
            else if (operandExpression.Type == typeof(double))
                return operandExpression.GetDoubleFloorCall();
            else
                throw new ArgumentException(nameof(operandExpression));
        }

        public static Expression GetDecimalFloorCall(this Expression operand)
            => Expression.Call(DecimalFloorMethodInfo, operand);

        public static Expression GetDoubleFloorCall(this Expression operand)
           => Expression.Call(DoubleFloorMethodInfo, operand);

        public static bool ByteArraysEqual(byte[] left, byte[] right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            if (left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                    return false;
            }

            return true;
        }

        public static bool ByteArraysNotEqual(byte[] left, byte[] right)
            => !ByteArraysEqual(left, right);

        internal static readonly MethodInfo EnumHasFlagMethodInfo = typeof(Enum).GetMethod("HasFlag", new[] { typeof(Enum) });
        internal static readonly MethodInfo StringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        internal static readonly MethodInfo StringConcatMethodInfo = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
        internal static readonly MethodInfo StringStartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        internal static readonly MethodInfo StringEndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        internal static readonly MethodInfo StringIndexOfMethodInfo = typeof(string).GetMethod("IndexOf", new[] { typeof(string) });
        internal static readonly MethodInfo StringSubstringStartMethodInfo = typeof(string).GetMethod("Substring", new[] { typeof(int) });
        internal static readonly MethodInfo StringSubstringStartFinishMethodInfo = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
        internal static readonly MethodInfo StringToLowerMethodInfo = typeof(string).GetMethod("ToLower", new Type[] { });
        internal static readonly MethodInfo StringToUpperMethodInfo = typeof(string).GetMethod("ToUpper", new Type[] { });
        internal static readonly MethodInfo StringTrimMethodInfo = typeof(string).GetMethod("Trim", new Type[] { });
        internal static readonly MemberInfo DateTimeMaxMemberInfo = typeof(DateTimeOffset).GetField("MaxValue");
        internal static readonly MemberInfo DateTimeMinMemberInfo = typeof(DateTimeOffset).GetField("MinValue");
        internal static readonly MemberInfo DateTimeUtcNowMemberInfo = typeof(DateTimeOffset).GetProperty("UtcNow");
        internal static readonly MethodInfo DecimalCeilingMethodInfo = typeof(Math).GetMethod("Ceiling", new Type[] { typeof(decimal) });
        internal static readonly MethodInfo DoubleCeilingMethodInfo = typeof(Math).GetMethod("Ceiling", new Type[] { typeof(double) });
        internal static readonly MethodInfo DecimalRoundMethodInfo = typeof(Math).GetMethod("Round", new Type[] { typeof(decimal) });
        internal static readonly MethodInfo DoubleRoundMethodInfo = typeof(Math).GetMethod("Round", new Type[] { typeof(double) });
        internal static readonly MethodInfo DecimalFloorMethodInfo = typeof(Math).GetMethod("Floor", new Type[] { typeof(decimal) });
        internal static readonly MethodInfo DoubleFloorMethodInfo = typeof(Math).GetMethod("Floor", new Type[] { typeof(double) });
        internal static readonly MethodInfo ToStringMethodInfo = typeof(object).GetMethod("ToString", new Type[] { });
        internal static readonly MethodInfo ByteArraysEqualMethodInfo = typeof(LinqHelpers).GetMethod("ByteArraysEqual", new Type[] { typeof(byte[]), typeof(byte[]) });
        internal static readonly MethodInfo ByteArraysNotEqualMethodInfo = typeof(LinqHelpers).GetMethod("ByteArraysNotEqual", new Type[] { typeof(byte[]), typeof(byte[]) });
        internal static readonly ConstructorInfo StringConstructorWithCharArrayParameters = typeof(string).GetConstructor(new[] { typeof(char[]) });
    }
}
