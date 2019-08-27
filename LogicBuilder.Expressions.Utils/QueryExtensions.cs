using LogicBuilder.Expressions.Utils.DataSource;
using LogicBuilder.Expressions.Utils.Strutures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace LogicBuilder.Expressions.Utils
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Creates an OrderBy expression from a SortCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortCollection"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<T>>> BuildOrderByExpression<T>(this SortCollection sortCollection) where T : class
        {
            if (sortCollection == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetOrderBy<T>(sortCollection);
            return Expression.Lambda<Func<IQueryable<T>, IQueryable<T>>>(mce, param);
        }

        /// <summary>
        /// Creates an order by method call expression to be invoked on an expression e.g. (parameter, member, method call) of type IQueryable<T>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <param name="sorts"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MethodCallExpression GetOrderBy<TSource>(this Expression expression, SortCollection sorts)
        {
            MethodCallExpression resultExp = sorts.SortDescriptions.Aggregate(null, (MethodCallExpression mce, SortDescription description) =>
            {
                LambdaExpression selectorExpression = description.PropertyName.GetTypedSelector<TSource>();
                MemberInfo orderByPropertyInfo = typeof(TSource).GetMemberInfoFromFullName(description.PropertyName);
                Type[] genericArgumentsForMethod = new Type[] { typeof(TSource), orderByPropertyInfo.GetMemberType() };

                if (mce == null)
                {//OrderBy and OrderByDescending espressions take two arguments each.  The parameter (object being extended by the helper method) and the lambda expression for the property selector
                    mce = description.SortDirection == ListSortDirection.Ascending
                        ? Expression.Call(typeof(Queryable), "OrderBy", genericArgumentsForMethod, expression, selectorExpression)
                        : Expression.Call(typeof(Queryable), "OrderByDescending", genericArgumentsForMethod, expression, selectorExpression);
                }
                else
                {//ThenBy and ThenByDescending espressions take two arguments each.  The resulting method call expression from OrderBy or OrderByDescending and the lambda expression for the property selector
                    mce = description.SortDirection == ListSortDirection.Ascending
                        ? Expression.Call(typeof(Queryable), "ThenBy", genericArgumentsForMethod, mce, selectorExpression)
                        : Expression.Call(typeof(Queryable), "ThenByDescending", genericArgumentsForMethod, mce, selectorExpression);
                }
                return mce;
            });

            resultExp = Expression.Call(typeof(Queryable), "Skip", new[] { typeof(TSource) }, resultExp, Expression.Constant(sorts.Skip));
            resultExp = Expression.Call(typeof(Queryable), "Take", new[] { typeof(TSource) }, resultExp, Expression.Constant(sorts.Take));

            return resultExp;
        }

        /// <summary>
        /// Creates an GroupBy expression from a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<IGrouping<object, T>>>> BuildGroupByExpression<T>(this string group) where T : class
        {
            if (group == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetGroupBy<T>(group);

            return Expression.Lambda<Func<IQueryable<T>, IQueryable<IGrouping<object, T>>>>(mce, param);
        }

        /// <summary>
        /// Creates a group by method call expression to be invoked on an expression e.g. (parameter, member, method call) of type IQueryable<T>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <param name="groupByProperty"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MethodCallExpression GetGroupBy<TSource>(this Expression expression, string groupByProperty)
        {
            LambdaExpression selectorExpression = groupByProperty.GetObjectSelector<TSource>();
            Type[] genericArgumentsForMethod = new Type[] { typeof(TSource), typeof(object) };

            return Expression.Call(typeof(Queryable), "GroupBy", genericArgumentsForMethod, expression, selectorExpression);
        }

        /// <summary>
        /// Creates a Where lambda expression from a filter group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<T>>> BuildWhereExpression<T>(this DataSource.FilterGroup group) where T : class
        {
            if (group == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetWhere<T>(group);

            return Expression.Lambda<Func<IQueryable<T>, IQueryable<T>>>(mce, param);
        }

        /// <summary>
        /// Creates a Where method call expression to be invoked on an expression e.g. (parameter, member, method call) of type IQueryable<T>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <param name="filterGroup"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MethodCallExpression GetWhere<TSource>(this Expression expression, DataSource.FilterGroup filterGroup) where TSource : class
        {
            LambdaExpression filterExpression = filterGroup.GetFilterExpression<TSource>();
            Type[] genericArgumentsForMethod = new Type[] { typeof(TSource) };

            return Expression.Call(typeof(Queryable), "Where", genericArgumentsForMethod, expression, filterExpression);
        }

        /// <summary>
        /// Creates a Where lambda expression from a filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<T>>> BuildWhereExpression<T>(this DataSource.Filter filter) where T : class
        {
            if (filter == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetWhere<T>(filter);

            return Expression.Lambda<Func<IQueryable<T>, IQueryable<T>>>(mce, param);
        }

        /// <summary>
        /// Creates a Where method call expression to be invoked on an expression e.g. (parameter, member, method call) of type IQueryable<T>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <param name="filter"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MethodCallExpression GetWhere<TSource>(this Expression expression, DataSource.Filter filter) where TSource : class
        {
            LambdaExpression filterExpression = filter.GetFilterExpression<TSource>();
            Type[] genericArgumentsForMethod = new Type[] { typeof(TSource) };

            return Expression.Call(typeof(Queryable), "Where", genericArgumentsForMethod, expression, filterExpression);
        }

        /// <summary>
        /// Function to create a lambda expression from a diverse group of method call expressions.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="param"></param>
        /// <param name="methodFunc"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, TDest>> BuildLambdaExpression<TSource, TDest>(this ParameterExpression param, Func<ParameterExpression, Expression> methodFunc)
            where TSource : class
            where TDest : class 
            => Expression.Lambda<Func<TSource, TDest>>(methodFunc(param), param);

        /// <summary>
        /// Create select new anonymous type using a dynamically created class called "AnonymousType" i.e. q => q.Select(p => new { ID = p.ID, FullName = p.FullName });
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyFullNames"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<dynamic>>> BuildSelectNewExpression<T>(this ICollection<string> propertyFullNames) where T : class
        {
            if (propertyFullNames == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetSelectNew<T>(propertyFullNames);
            return Expression.Lambda<Func<IQueryable<T>, IQueryable<dynamic>>>(mce, param);
        }

        public static MethodCallExpression GetSelectNew<TSource>(this Expression expression, ICollection<string> propertyFullNames, string parameterName = "a") where TSource : class
        {
            return expression.GetSelectMethodExpression<TSource>
                (
                    //gets the flattened name for the anonymous type if necessary
                    GetMemberDetails<TSource>(propertyFullNames),
                    //Parameter for the selector 
                    Expression.Parameter(typeof(TSource), parameterName)
                );
        }

        /// <summary>
        /// Creates Distinct method call expression to run against a queryable
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetDistinct(this Expression expression) => expression.GetMethodCall("Distinct");

        /// <summary>
        /// Creates Single method call expression to run against a queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetSingle(this Expression expression) => expression.GetMethodCall("Single");

        /// <summary>
        /// Creates SingleOrDefault method call expression to run against a queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetSingleOrDefault(this Expression expression) => expression.GetMethodCall("SingleOrDefault");

        /// <summary>
        /// Creates First method call expression to run against a queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetFirst(this Expression expression) => expression.GetMethodCall("First");

        /// <summary>
        /// Creates FirstOrDefault method call expression to run against a queryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression GetFirstOrDefault(this Expression expression) => expression.GetMethodCall("FirstOrDefault");

        private static MethodCallExpression GetMethodCall(this Expression expression, string methodName)
            => Expression.Call(typeof(Queryable), methodName, new Type[] { expression.Type.GetUnderlyingElementType() }, expression);

        /// <summary>
        /// Create select new anonymous type using a dynamically created class called "AnonymousType" i.e. q => q.Select(p => new { ID = p.ID, FullName = p.FullName });
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyFullNames"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<dynamic>>> BuildSelectNewExpression<T>(this IDictionary<string, string> propertyFullNames) where T : class
        {
            if (propertyFullNames == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetSelectNew<T>(propertyFullNames);
            return Expression.Lambda<Func<IQueryable<T>, IQueryable<dynamic>>>(mce, param);
        }

        public static MethodCallExpression GetSelectNew<TSource>(this Expression expression, IDictionary<string, string> propertyFullNames, string parameterName = "a") where TSource : class
        {
            return expression.GetSelectMethodExpression<TSource>
                (
                    GetMemberDetails<TSource>(propertyFullNames),
                    Expression.Parameter(typeof(TSource), parameterName)//Parameter for the selector 
                );
        }

        private static MethodCallExpression GetSelectMethodExpression<TSource>(this Expression expression, List<MemberDetails> memberDetails, ParameterExpression param)
        {
            //anonymous type
            Type anonymousType = CreateAnonymousType(memberDetails);

            //Bind anonymous type's member to TSource's selector.
            IEnumerable<MemberBinding> bindings = memberDetails.Select
            (
                nameType => Expression.Bind
                (
                    //PropertyInfo for the anonymous type's member
                    anonymousType.GetMemberInfoFromFullName(nameType.MemberName),
                    //Selector expression for the TSource member
                    nameType.SourceFullName.Split('.').Aggregate((Expression)param, (p, n) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(n)))
                )
            );

            //Func<TSource, anonymous> s => new AnonymousType { Member = s.Member }
            LambdaExpression selectorExpression = Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType(new Type[] { typeof(TSource), anonymousType }),
                Expression.MemberInit(Expression.New(anonymousType), bindings),
                param
            );

            //IQueryable<anonymousType> Select<TSource, anonymousType>(this IQueryable<TSource> source, Expression<Func<TSource, anonymousType>> selector);
            return Expression.Call(typeof(Queryable), "Select", new Type[] { typeof(TSource), anonymousType }, expression, selectorExpression);
        }

        private class MemberDetails
        {
            public string SourceFullName { get; set; }
            public string MemberName { get; set; }
            public Type Type { get; set; }
        }

        private static List<MemberDetails> GetMemberDetails<TSource>(IDictionary<string, string> propertyFullNames)
            => propertyFullNames.Aggregate(new List<MemberDetails>(), (list, next) =>
            {
                Type t = typeof(TSource);
                List<string> fullNameList = next.Value.Split('.').Aggregate(new List<string>(), (lst, n) =>
                {
                    MemberInfo p = t.GetMemberInfo(n);
                    t = p.GetMemberType();
                    lst.Add(p.Name);
                    return lst;
                });

                list.Add(new MemberDetails
                {
                    SourceFullName = string.Join(".", fullNameList),
                    MemberName = next.Key,
                    Type = t
                });
                return list;
            });

        private static List<MemberDetails> GetMemberDetails<TSource>(ICollection<string> propertyFullNames)
            => propertyFullNames.Aggregate(new List<MemberDetails>(), (list, next) =>
            {
                Type t = typeof(TSource);
                List<string> fullNameList = next.Split('.').Aggregate(new List<string>(), (lst, n) =>
                {
                    MemberInfo p = t.GetMemberInfo(n);
                    t = p.GetMemberType();
                    lst.Add(p.Name);
                    return lst;
                });

                list.Add(new MemberDetails
                {
                    SourceFullName = string.Join(".", fullNameList),
                    MemberName = string.Join("", fullNameList),
                    Type = t
                });
                return list;
            });

        private static Type CreateAnonymousType(IEnumerable<MemberDetails> memberDetails)
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("TempAssembly");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("TempAssembly");
            TypeBuilder typeBuilder = dynamicModule.DefineType("AnonymousType", TypeAttributes.Public);
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            var builders = memberDetails.Select(info => new
            {
                FieldBuilder = typeBuilder.DefineField(string.Concat("_", info.MemberName), info.Type, FieldAttributes.Private),
                PropertyBuilder = typeBuilder.DefineProperty(info.MemberName, PropertyAttributes.HasDefault, info.Type, null),
                GetMethodBuilder = typeBuilder.DefineMethod(string.Concat("get_", info.MemberName), getSetAttr, info.Type, Type.EmptyTypes),
                SetMethodBuilder = typeBuilder.DefineMethod(string.Concat("set_", info.MemberName), getSetAttr, null, new Type[] { info.Type })
            });

            builders.ToList().ForEach(builder =>
            {
                ILGenerator getMethodIL = builder.GetMethodBuilder.GetILGenerator();
                getMethodIL.Emit(OpCodes.Ldarg_0);
                getMethodIL.Emit(OpCodes.Ldfld, builder.FieldBuilder);
                getMethodIL.Emit(OpCodes.Ret);

                ILGenerator setMethodIL = builder.SetMethodBuilder.GetILGenerator();
                setMethodIL.Emit(OpCodes.Ldarg_0);
                setMethodIL.Emit(OpCodes.Ldarg_1);
                setMethodIL.Emit(OpCodes.Stfld, builder.FieldBuilder);
                setMethodIL.Emit(OpCodes.Ret);

                builder.PropertyBuilder.SetGetMethod(builder.GetMethodBuilder);
                builder.PropertyBuilder.SetSetMethod(builder.SetMethodBuilder);
            });

            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        /// Create a dictionary select from a list of properties in lieu of select new anonymous type.   New requires IL code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyFullNames"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<T>, IQueryable<Dictionary<string, object>>>> BuildSelectDictionaryExpression<T>(this ICollection<string> propertyFullNames) where T : class
        {
            if (propertyFullNames == null)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<T>), "q");
            MethodCallExpression mce = param.GetSelectDictionary<T>(propertyFullNames);
            return Expression.Lambda<Func<IQueryable<T>, IQueryable<Dictionary<string, object>>>>(mce, param);
        }

        /// <summary>
        /// Creates a select dictionary method call expression to be invoked on an expression e.g. (parameter, member, method call) of type IQueryable<T> in lieu of select new anonymous type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="expression"></param>
        /// <param name="propertyFullNames"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static MethodCallExpression GetSelectDictionary<TSource>(this Expression expression, ICollection<string> propertyFullNames, string parameterName = "a")
        {
            List<LambdaExpression> selectors = propertyFullNames.Aggregate(new List<LambdaExpression>(), (mems, next) => {
                mems.Add(next.GetTypedSelector<TSource>(parameterName));
                return mems;
            });

            ParameterExpression param = Expression.Parameter(typeof(TSource), parameterName);

            List<KeyValuePair<string, Expression>> dictionaryInitializers = propertyFullNames.Aggregate(new List<KeyValuePair<string, Expression>>(), (mems, next) => {
                string[] parts = next.Split('.');
                Expression parent = parts.Aggregate((Expression)param, (p, n) => Expression.MakeMemberAccess(p, p.Type.GetMemberInfo(n)));
                if (parent.Type.GetTypeInfo().IsValueType)//Convert value type expressions to object expressions otherwise
                    parent = Expression.Convert(parent, typeof(object));//Expression.Lambda below will throw an exception for value types

                mems.Add(new KeyValuePair<string, Expression>(next, parent));
                return mems;
            });

            //Dictionary<string, object>.Add
            MethodInfo addMethod = typeof(Dictionary<string, object>).GetMethod(
                "Add", new[] { typeof(string), typeof(object) });
            //Create a Dictionary here. Each entry is a single propperty and lambda expression to create the value
            ListInitExpression createDictionaryEntrySelector = Expression.ListInit(
                    Expression.New(typeof(Dictionary<string, object>)),
                    dictionaryInitializers.Select(kvp => Expression.ElementInit(addMethod, new Expression[] { Expression.Constant(kvp.Key), kvp.Value })));

            //Func<TSource, Dictionary<string, object>>
            LambdaExpression selectorExpression = Expression.Lambda<Func<TSource, Dictionary<string, object>>>(
                createDictionaryEntrySelector,
                param);

            Type[] genericArgumentsForSelectMethod = new Type[] { typeof(TSource), typeof(Dictionary<string, object>) };

            return Expression.Call(typeof(Queryable), "Select", genericArgumentsForSelectMethod, expression, selectorExpression);
        }

        /// <summary>
        /// Creates a list of navigation expressions from the list of period delimited navigation properties.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="includes"></param>
        /// <returns></returns>
        public static IEnumerable<Expression<Func<TSource, object>>> BuildIncludes<TSource>(this IEnumerable<string> includes)
            where TSource : class
            => includes.Select(include => BuildSelectorExpression<TSource>(include)).ToList();

        /// <summary>
        /// Build Selector Expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="fullName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, object>> BuildSelectorExpression<TSource>(string fullName, string parameterName = "i")
            => (Expression<Func<TSource, object>>)BuildSelectorExpression(typeof(TSource), fullName, parameterName);

        /// <summary>
        /// Build Selector Expression
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fullName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static LambdaExpression BuildSelectorExpression(Type type, string fullName, string parameterName = "i")
        {
            ParameterExpression param = Expression.Parameter(type, parameterName);
            string[] parts = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type parentType = type;
            Expression parent = param;

            for (int i = 0; i < parts.Length; i++)
            {
                if (parentType.IsList())
                {
                    parent = GetSelectExpression(parts.Skip(i), parent, parentType.GetUnderlyingElementType(), parameterName);//parentType is the underlying type of the member since it is an IEnumerable<T>
                    return Expression.Lambda
                    (
                        typeof(Func<,>).MakeGenericType(new[] { type, typeof(object) }),
                        parent,
                        param
                    );
                }
                else
                {
                    MemberInfo mInfo = parentType.GetMemberInfo(parts[i]);
                    parent = Expression.MakeMemberAccess(parent, mInfo);

                    parentType = mInfo.GetMemberType();
                }
            }

            if (parent.Type.IsValueType)//Convert value type expressions to object expressions otherwise
                parent = Expression.Convert(parent, typeof(object));//Expression.Lambda below will throw an exception for value types

            return Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType(new[] { type, typeof(object) }),
                parent,
                param
            );
        }

        private static string ChildParameterName(this string currentParameterName)
        {
            string lastChar = currentParameterName.Substring(currentParameterName.Length - 1);
            if (short.TryParse(lastChar, out short lastCharShort))
            {
                return string.Concat(currentParameterName.Substring(0, currentParameterName.Length - 1), (lastCharShort++).ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                return currentParameterName += "0";
            }
        }

        private static Expression GetSelectExpression(IEnumerable<string> parts, Expression parent, Type underlyingType, string parameterName)//underlying type because paranet is a collection
            => Expression.Call
            (
                typeof(Enumerable),//This is an Enumerable (not Queryable) select.  We are selecting includes for a member who is a collection
                "Select",
                new Type[] { underlyingType, typeof(object) },
                parent,
                BuildSelectorExpression(underlyingType, string.Join(".", parts), parameterName.ChildParameterName())//Join the remaining parts to create a full name
            );
    }
}
