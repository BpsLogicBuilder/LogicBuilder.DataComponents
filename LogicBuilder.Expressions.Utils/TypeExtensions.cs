using LogicBuilder.Expressions.Utils.Properties;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns the property info for a type given the full property name (supports granchild properties)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyFullName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns MemberInfo given the parent type and member name.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo(this Type parentType, string memberName)
        {
            MemberInfo mInfo = parentType.GetMember(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase).FirstOrDefault();
            if (mInfo == null)
                throw new ArgumentException(string.Format(Resources.invalidPropertyFormat, memberName, parentType.FullName));

            return mInfo;
        }

        /// <summary>
        /// Returns the System.Type for memberInfo
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the System.Type for member expressions member.
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberExpression me)
            => me.Member.GetMemberType();

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
        {
            TypeInfo tInfo = expression.Type.GetTypeInfo();
            if (tInfo.IsArray)
                return tInfo.GetElementType();

            Type[] genericArguments;
            if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
                throw new ArgumentException("type");

            return genericArguments[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsList(this Type type)
            => type.IsArray || (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type));

        /// <summary>
        /// Get the member type or its the underlying element type if it is a list
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public static Type GetCurrentType(this Type memberType)
            //when the member is an IEnumberable<T> we really need T.
            => memberType.IsList()
                ? memberType.GetUnderlyingElementType()
                : memberType;
    }
}
