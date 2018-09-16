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

        ///// <summary>
        ///// Returns the property info given the name of an immediate child property.
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //public static MemberInfo FindProperty(this Type type, string propertyName)
        //{

        //    PropertyInfo propertyInfo = type.GetProperty(propertyName);
        //    if (propertyInfo == null)
        //        propertyInfo = type.GetProperty(propertyName.ToPascalCase());

        //    if (propertyInfo == null)
        //        propertyInfo = type.GetProperty(propertyName.ToCamelCase());

        //    if (propertyInfo == null)
        //        propertyInfo = type.GetProperties().SingleOrDefault(p => p.Name.ToLowerInvariant() == propertyName.ToLowerInvariant());

        //    if (propertyInfo == null)
        //        throw new ArgumentException(string.Format(Resources.invalidPropertyFormat, propertyName, type.FullName));

        //    return propertyInfo;
        //}

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

        internal static Type GetUnderlyingElementType(this Type type)
        {
            TypeInfo tInfo = type.GetTypeInfo();
            Type[] genericArguments;
            if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
                throw new ArgumentException("type");

            return genericArguments[0];
        }

        internal static Type GetUnderlyingElementType(this Expression expression)
        {
            TypeInfo tInfo = expression.Type.GetTypeInfo();
            Type[] genericArguments;
            if (!tInfo.IsGenericType || (genericArguments = tInfo.GetGenericArguments()).Length != 1)
                throw new ArgumentException("type");

            return genericArguments[0];
        }
    }
}
