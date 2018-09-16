using LogicBuilder.Kendo.ExpressionExtensions.Resources;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Kendo.ExpressionExtensions.Extensions
{
    internal static class TypeExtensionsEx
    {
        //internal static Type GetUnderlyingElementType(this Type type)
        //{
        //    TypeInfo tInfo = type.GetTypeInfo();
        //    Type[] genericArguments;
        //    if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
        //        throw new ArgumentException("type");

        //    return genericArguments[0];
        //}

        internal static Type GetUnderlyingElementType(this Expression expression)
        {
            TypeInfo tInfo = expression.Type.GetTypeInfo();
            Type[] genericArguments;
            if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
                throw new ArgumentException("type");

            return genericArguments[0];
        }

        internal static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        internal static string FirstSortableProperty(this Type type)
        {
            PropertyInfo firstSortableProperty = type.GetProperties().Where(property => property.PropertyType.IsPredefinedType()).FirstOrDefault();

            if (firstSortableProperty == null)
            {
                throw new NotSupportedException(Exceptions.CannotFindPropertyToSortBy);
            }

            return firstSortableProperty.Name;
        }

        internal static readonly Type[] PredefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        internal static bool IsPredefinedType(this Type type)
        {
            foreach (Type t in PredefinedTypes)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
