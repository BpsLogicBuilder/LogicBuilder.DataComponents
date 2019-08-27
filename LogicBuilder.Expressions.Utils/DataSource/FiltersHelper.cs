using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    public static class FiltersHelper
    {
        private static readonly IDictionary<string, string> operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"},
            {"isnotempty", "!="},
            {"isempty", "="},
            {"isnotnull", "!="},
            {"isnull", "="}
        };

        public static List<Filter> GetAll(this FilterGroup filterGroup)
        {
            if (filterGroup == null)
                return null;

            List<Filter> filters = new List<Filter>();
            filterGroup.GetAll(filters);
            return filters;
        }

        public static void GetAll(this FilterGroup filterGroup, List<Filter> filters)
        {
            if (filterGroup == null)
                return;

            if (filters == null) throw new ArgumentException("Filters cannot be null");

            if (filterGroup.Filters != null && filterGroup.Filters.Any())
            {
                foreach (Filter f in filterGroup.Filters)
                    f.GetAll(filters);
            }

            if (filterGroup.FilterGroups != null && filterGroup.FilterGroups.Any())
            {
                foreach (FilterGroup f in filterGroup.FilterGroups)
                    f.GetAll(filters);
            }
        }

        public static List<Filter> GetAll(this Filter filter)
        {
            if (filter == null)
                return null;

            List<Filter> filters = new List<Filter>();
            filter.GetAll(filters);
            return filters;
        }

        private static void GetAll(this Filter filter, List<Filter> filters)
        {
            if (filter == null)
                return;

            if (filter.Operator != null)
                filters.Add(filter);

            if (filters == null) throw new ArgumentException("Filters cannot be null");

            if (filter.Filters != null && filter.Filters.Any())
            {
                foreach (Filter f in filter.Filters)
                    f.GetAll(filters);
            }
        }

        /// <summary>
        /// Adapted from Kendo.DynamicLinq.Filter.ToExpression(IList<Filter> filters)
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ToExpression<T>(this Filter filter, List<Filter> filters, string parameterName, List<object> values, bool checkForNull = false) where T : class
        {
            if (filter.Filters != null && filter.Filters.Any())
            {
                return "(" + String.Join(" " + filter.Logic + " ", filter.Filters.Select(fil => fil.ToExpression<T>(filters, parameterName, values, checkForNull)).ToArray()) + ")";
            }

            //reset field name for case
            filter.Field = filter.Field.GetMemberExpression<T>().GetFullName();
            const string PERIOD = ".";
            string field = checkForNull
                ? filter.Field.CheckForNull<T>(parameterName, values)
                : string.Concat(parameterName, PERIOD, filter.Field);

            int index = filters.IndexOf(filter);

            string comparison = operators[filter.Operator];

            if (filter.Operator == "doesnotcontain")
                return String.Format("!{0}.{1}(@{2})", field, comparison, index);

            if (filter.Operator == "startswith" || filter.Operator == "endswith" || filter.Operator == "contains")
                return String.Format("{0}.{1}(@{2})", field, comparison, index);

            if (filter.Operator == "isnotnull" || filter.Operator == "isnull")
                filter.Value = null;
            if (filter.Operator == "isnotempty" || filter.Operator == "isempty")
                filter.Value = string.Empty;

            return String.Format("{0} {1} @{2}", field, comparison, index);
        }

        /// <summary>
        /// Adapted from Kendo.DynamicLinq.Filter.ToExpression(IList<Filter> filters)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterGroup"></param>
        /// <param name="filters"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="checkForNull"></param>
        /// <returns></returns>
        public static string ToExpression<T>(this FilterGroup filterGroup, List<Filter> filters, string parameterName, List<object> values, bool checkForNull = false) where T : class
        {
            List<string> expressions = new List<string>();
            if (filterGroup.Filters != null && filterGroup.Filters.Any())
                expressions.AddRange(filterGroup.Filters.Select(fil => fil.ToExpression<T>(filters, parameterName, values, checkForNull)).ToArray());
                
            if (filterGroup.FilterGroups != null && filterGroup.FilterGroups.Any())
                expressions.AddRange(filterGroup.FilterGroups.Select(fil => fil.ToExpression<T>(filters, parameterName, values, checkForNull)).ToArray());

            return "(" + String.Join(" " + filterGroup.Logic + " ", expressions) + ")";
        }

        private static string CheckForNull<T>(this string fieldFullName, string parameterName, List<object> values)
        {
            const string PERIOD = ".";
            MemberInfo pInfo = typeof(T).GetMemberInfo(fieldFullName);
            string[] parts = fieldFullName.Split(PERIOD[0]);

            string currentName = parameterName;
            string check = string.Concat(currentName, " != null");
            StringBuilder sb = new StringBuilder(check);
            List<string> partNames = new List<string>();

            foreach (string part in parts)
            {
                partNames.Add(part);
                MemberInfo propertyInfo = typeof(T).GetMemberInfoFromFullName(string.Join(PERIOD, partNames));
                if (propertyInfo.GetMemberType().GetTypeInfo().IsValueType && Activator.CreateInstance(propertyInfo.GetMemberType()) != null)
                    continue;//If check for null then check for null on nullable types only

                currentName = string.Concat(currentName, PERIOD, part);
                sb.Append(" && ");
                check = string.Concat(currentName, " != null");
                sb.Append(check);
            }

            dynamic val = pInfo.GetMemberType() == typeof(string)
                ? ""
                : Activator.CreateInstance(GetValueType(pInfo.GetMemberType()));

            string field = string.Concat(parameterName, PERIOD, fieldFullName);
            if (pInfo.GetMemberType().GetTypeInfo().IsValueType && Activator.CreateInstance(pInfo.GetMemberType()) != null)//If check for null then check for null on nullable types only
            {
                field = string.Concat(sb.ToString(), " && ", field);
            }
            else
            {
                values.Add(val);
                field = pInfo.GetMemberType().ToString() == "System.String"
                    ? string.Format("(IIF(({0}), {1}, null) ?? @{2})", sb.ToString(), currentName, values.Count - 1)
                    : string.Format("(IIF(({0}), {1}, @{2}))", sb.ToString(), currentName, values.Count - 1);
            }

            return field;
        }

        private static Type GetValueType(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                return Nullable.GetUnderlyingType(type);
            else
                return type;
        }

        /// <summary>
        /// Creates a filter expression from the filter object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetFilterExpression<T>(this Filter filter, string parameterName = "f") where T : class
        {
            if (filter == null || (filter.Operator == null && filter.Filters.Count() == 0))
                return null;

            List<Filter> filters = filter.GetAll();
            List<object> values = filters.Select(f =>
            {
                MemberInfo pInfo = typeof(T).GetMemberInfoFromFullName(f.Field);
                if (f.Value != null && pInfo.GetMemberType() != f.Value.GetType())//convert values if necessary
                    f.Value = Convert.ChangeType(f.Value, pInfo.GetMemberType());//NewtonSoft Json defaults to long for whole numbers and double for fractions

                return f.Value;
            }).ToList();

            string expression = filter.ToExpression<T>(filters, parameterName, values, false);

            return System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(false,
                new ParameterExpression[] { Expression.Parameter(typeof(T), parameterName) },
                typeof(bool),
                expression,
                values.ToArray()) as Expression<Func<T, bool>>;
        }

        public static LambdaExpression GetFilterExpression(this FilterGroup filterGroup, Type type, string parameterName = "f") 
            => (LambdaExpression)"_GetFilterExpression"
            .GetMethod()
            .MakeGenericMethod(type)
            .Invoke(null, new object[] { filterGroup, parameterName });

        private static MethodInfo GetMethod(this string methodName)
           => typeof(FiltersHelper).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

        private static Expression<Func<T, bool>> _GetFilterExpression<T>(this FilterGroup filterGroup, string parameterName = "f") where T : class
            => filterGroup.GetFilterExpression<T>(parameterName);

        /// <summary>
        /// Creates a filter expression from a filter group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterGroup"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetFilterExpression<T>(this FilterGroup filterGroup, string parameterName = "f") where T : class
        {
            if (filterGroup == null || 
                ((filterGroup.FilterGroups == null || filterGroup.FilterGroups.Count() == 0) && (filterGroup.Filters == null || filterGroup.Filters.Count() == 0)))
                return null;

            List<Filter> filters = filterGroup.GetAll();
            List<object> values = filters.Select(f =>
            {
                MemberInfo pInfo = typeof(T).GetMemberInfoFromFullName(f.Field);
                if (f.Value != null && pInfo.GetMemberType() != f.Value.GetType())//convert values if necessary
                    f.Value = Convert.ChangeType(f.Value, pInfo.GetMemberType());//NewtonSoft Json defaults to long for whole numbers and double for fractions

                return f.Value;
            }).ToList();

            return System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda
            (
                false,
                new ParameterExpression[] { Expression.Parameter(typeof(T), parameterName) },
                typeof(bool),
                filterGroup.ToExpression<T>(filters, parameterName, values, false),
                values.ToArray()
            ) as Expression<Func<T, bool>>;
        }
    }
}
