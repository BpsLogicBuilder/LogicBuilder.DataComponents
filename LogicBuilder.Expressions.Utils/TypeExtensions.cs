using LogicBuilder.Expressions.Utils.Properties;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils
{
    public static class TypeExtensions
    {
        const BindingFlags instanceBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static MemberInfo GetMemberInfoFromFullName(this Type type, string propertyFullName)
        {
            if (propertyFullName.IndexOf('.') < 0)
            {
                return type.GetMemberInfo(propertyFullName);
            }

            string propertyName = propertyFullName.Substring(0, propertyFullName.IndexOf('.'));
            string childFullName = propertyFullName.Substring(propertyFullName.IndexOf('.') + 1);

            return GetMemberInfoFromFullName(type.GetMemberInfo(propertyName).GetMemberType(), childFullName);
        }

        public static MemberInfo GetMemberInfo(this Type parentType, string memberName)
        {
            MemberInfo mInfo = parentType.GetMember(memberName, instanceBindingFlags).FirstOrDefault();

            //AutoMapper's expansions use declared only MemberInfos
            if (mInfo?.DeclaringType != null && mInfo.DeclaringType.FullName != parentType.FullName)
                mInfo = mInfo.DeclaringType.GetMember(memberName, instanceBindingFlags).FirstOrDefault();

            if (mInfo == null)
                throw new ArgumentException(string.Format(Resources.invalidPropertyFormat, memberName, parentType.FullName));

            return mInfo;
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case MethodInfo mInfo:
                    return mInfo.ReturnType;
                case PropertyInfo pInfo:
                    return pInfo.PropertyType;
                case FieldInfo fInfo:
                    return fInfo.FieldType;
                case null:
                    throw new ArgumentNullException(nameof(memberInfo));
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberInfo));
            }
        }

        public static bool IsLiteralType(this Type type)
        {
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);

            return LiteralTypes.Contains(type) 
                || Net6OnlyLiteralTypes.Contains(type.FullName) 
                || typeof(Enum).IsAssignableFrom(type);
        }

        private static HashSet<Type> LiteralTypes => new HashSet<Type>(_literalTypes);

        private static readonly HashSet<string> Net6OnlyLiteralTypes = new()
        {
            NET6OnlyLiteralTypeNames.DATEONLY,
            NET6OnlyLiteralTypeNames.TIMEONLY
        };

        private static Type[] _literalTypes => new Type[] {
                typeof(bool),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(Date),
                typeof(TimeSpan),
                typeof(TimeOfDay),
                typeof(Guid),
                typeof(decimal),
                typeof(byte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(char),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(string)
            };

        public static Type GetMemberType(this MemberExpression me)
            => me.Member.GetMemberType();

        public static bool IsNullableType(this Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool TryParseEnum(this string toParse, Type enumType, out object result)
        {
            if (!typeof(Enum).IsAssignableFrom(enumType))
                throw new ArgumentException(nameof(enumType));

            MethodInfo method = GetMethod();
            Type underlyingType = Nullable.GetUnderlyingType(enumType) ?? enumType;

            if (method == null)
            {
                result = GetResult();
                return false;
            }

            object[] args = new object[] { toParse, null };
            bool success = (bool)method.MakeGenericMethod(underlyingType).Invoke(null, args);
            result = success ? args[1] : GetResult();

            return success;

            object GetResult()
                => enumType.IsValueType && !enumType.IsNullableType() ? Activator.CreateInstance(underlyingType) : null;

            static MethodInfo GetMethod()
                => typeof(Enum).GetMethods().SingleOrDefault(IsTryParseMethod);

            static bool IsTryParseMethod(MethodInfo method)
            {
                if (method.Name != "TryParse") return false;
                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 2
                    && parameters[0].ParameterType == typeof(string)
                    && parameters[1].IsOut;
            }
        }

        public static Type ToNullableUnderlyingType(this Type type)
        {
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);

            return type;
        }

        public static Type ToNullable(this Type type)
        {
            if (type.IsNullableType() || !type.IsValueType)
                return type;

            return typeof(Nullable<>).MakeGenericType(type);
        }

        public static Type GetUnderlyingElementType(this Type type)
        {
            TypeInfo tInfo = type.GetTypeInfo();
            if (tInfo.IsArray)
                return tInfo.GetElementType();

            Type[] genericArguments;
            if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
                throw new ArgumentException("type");

            return genericArguments[0];
        }

        public static Type GetUnderlyingElementType(this Expression expression)
            => expression.Type.GetUnderlyingElementType();

        public static bool IsList(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGrouping<,>))
                return false;

            return type.IsArray || (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type));
        }

        public static bool IsIQueryable(this Type type)
            => type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type);

        public static Type GetCurrentType(this Type memberType)
            //when the member is an IEnumberable<T> we really need T.
            => memberType.IsList()
                ? memberType.GetUnderlyingElementType()
                : memberType;

        public static MemberInfo[] GetSelectedMembers(this Type parentType, List<string> selects)
        {
            if (selects == null || !selects.Any())
                return parentType.GetValueTypeMembers();

            return selects.Select(select => parentType.GetMemberInfo(select)).ToArray();
        }

        private static MemberInfo[] GetValueTypeMembers(this Type parentType)
        {
            if (parentType.IsList())
                return new MemberInfo[] { };

            return parentType.GetMemberInfos().Where
            (
                info => (info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property)
                && (info.GetMemberType().IsLiteralType() || info.GetMemberType() == typeof(byte[]))//Need typeof(byte[]) for SQL Server timestamp column
            ).ToArray();
        }

        private static MemberInfo[] GetMemberInfos(this Type parentType) 
            => parentType.GetMembers(instanceBindingFlags).Select
            (
                member =>
                {
                    if (member.DeclaringType != parentType)
                        return member.DeclaringType.GetMember(member.Name, instanceBindingFlags).FirstOrDefault();

                    return member;
                }
            ).ToArray();
    }
}
